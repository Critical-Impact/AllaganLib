using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AllaganLib.Universalis.Models.Bson;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace AllaganLib.Universalis.Services;

public class UniversalisWebsocketService : BackgroundService
{
    public const uint BackoffLimit = 10;
    public const uint BackoffSeconds = 20;

    private ClientWebSocket client;
    private readonly Func<ClientWebSocket> websocketFactory;
    private readonly IPluginLog pluginLog;
    private readonly Uri uri = new("wss://universalis.app/api/ws");
    private readonly ConcurrentQueue<(EventType, uint)> subscriptionChannelQueue = new();
    private readonly ConcurrentQueue<(EventType, uint)> unsubscriptionChannelQueue = new();
    private readonly HashSet<(EventType, uint)> subscriptions = [];
    private readonly Dictionary<uint, uint> backoffCounts = new();

    public UniversalisWebsocketService(ClientWebSocket client, Func<ClientWebSocket> websocketFactory, IPluginLog pluginLog)
    {
        this.client = client;
        this.websocketFactory = websocketFactory;
        this.pluginLog = pluginLog;
    }

    private bool AutoResubscribe { get; set; } = true;

    private bool Enabled { get; set; } = true;

    public ClientWebSocket Client => this.client;

    public delegate void UniversalisEventDelegate(SubscriptionReceivedMessage subscriptionReceivedMessage);

    public event UniversalisEventDelegate? OnUniversalisEvent;

    public enum EventType
    {
        ListingsAdd = 0,
        ListingsRemove = 1,
        SalesAdd = 2,
        SalesRemove = 3,
        Unknown = 4
    }

    public void SubscribeToChannel(EventType eventType, uint worldId)
    {
        var newSubscription = (subscriptionType: eventType, worldId);
        if (!this.subscriptions.Contains(newSubscription))
        {
            this.pluginLog.Verbose($"Subscribing to {eventType} for world: {worldId}");
            this.subscriptionChannelQueue.Enqueue(newSubscription);
        }
    }

    public void UnsubscribeFromChannel(EventType eventType, uint worldId)
    {
        var newSubscription = (subscriptionType: eventType, worldId);
        if (this.subscriptions.Contains(newSubscription))
        {
            this.pluginLog.Verbose($"Unsubscribing from {eventType} for world: {worldId}");
            this.unsubscriptionChannelQueue.Enqueue(newSubscription);
        }
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAny(this.SendingLoop(cancellationToken), this.ReceivingLoop(cancellationToken));
    }

    /// <summary>
    /// Either connects to universalis, subscribes, unsubscribes or receives data
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    protected async Task SendingLoop(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (!this.Enabled && this.client.State == WebSocketState.Open)
            {
                await this.client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
            }

            if (!this.Enabled)
            {
                await Task.Delay(1000, cancellationToken);
                continue;
            }

            if (this.client.State == WebSocketState.Connecting)
            {
                await Task.Delay(100, cancellationToken);
                continue;
            }

            if (this.Client.State is WebSocketState.None or WebSocketState.Closed)
            {
                if (this.subscriptionChannelQueue.Count != 0)
                {
                    await this.ConnectWithRetryAsync(cancellationToken);
                    await Task.Delay(1000, cancellationToken);
                }
                else if (this.subscriptions.Count != 0)
                {
                    this.pluginLog.Verbose($"Client was disconnected but subscriptions are present. Disconnect likely, waiting 5 seconds before reconnecting.");
                    await Task.Delay(5000, cancellationToken);
                    await this.ConnectWithRetryAsync(cancellationToken);
                }
                else
                {
                    await Task.Delay(500, cancellationToken);
                }

                continue;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (this.subscriptionChannelQueue.Count != 0)
            {
                await this.SendMessages(this.subscriptionChannelQueue, "subscribe", cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (this.unsubscriptionChannelQueue.Count != 0)
            {
                await this.SendMessages(this.unsubscriptionChannelQueue, "unsubscribe", cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (this.subscriptions.Count == 0)
            {
                // No subscribed worlds, wait a second and check again
                await Task.Delay(1000, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            // Nothing to do
            await Task.Delay(100, cancellationToken);
        }
    }

    /// <summary>
    /// Receives data
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    protected async Task ReceivingLoop(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (this.Client.State is WebSocketState.None or WebSocketState.Closed or WebSocketState.Connecting or WebSocketState.CloseReceived or WebSocketState.CloseSent)
            {
                await Task.Delay(500, cancellationToken);
                continue;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await Task.Delay(100, cancellationToken);

            await this.ReceiveMessages(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task SendMessages(
        ConcurrentQueue<(EventType SubscriptionType, uint WorldId)> subscriptionQueue,
        string eventName,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (subscriptionQueue.Count == 0)
        {
            return;
        }

        if (!subscriptionQueue.TryDequeue(out var nextMessage))
        {
            return;
        }
        var worldId = nextMessage.WorldId;
        string channelType;
        switch (nextMessage.SubscriptionType)
        {
            case EventType.ListingsAdd:
                channelType = "listings/add";
                break;
            case EventType.ListingsRemove:
                channelType = "listings/remove";
                break;
            case EventType.SalesAdd:
                channelType = "sales/add";
                break;
            case EventType.SalesRemove:
                channelType = "sales/remove";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var channel = channelType + "{world=" + worldId + "}";
        var subscriptionMessage = new SubscriptionMessage(eventName, channel).ToBsonDocument();
        var segment = new ArraySegment<byte>(subscriptionMessage.ToBson());

        try
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await this.Client.SendAsync(segment, WebSocketMessageType.Binary, false, cancellationToken);
                if (eventName == "subscribe")
                {
                    this.subscriptions.Add(nextMessage);
                    this.pluginLog.Verbose($"Subscribed to world {worldId}'s {channelType}.");
                }
                else
                {
                    this.subscriptions.Remove(nextMessage);
                    this.pluginLog.Verbose($"Unsubscribed from world {worldId}'s {channelType}.");
                }
            }
        }
        catch (Exception ex)
        {
            if (this.backoffCounts.TryGetValue(worldId, out var count))
            {
                count++;
            }
            else
            {
                count = 1;
            }

            this.pluginLog.Verbose($"Failed to subscribe to world {worldId}'s {channelType}. This has occured {count} times. Backing off for {BackoffSeconds} seconds", ex);
            if (count >= BackoffLimit)
            {
                this.pluginLog.Verbose($"Reached back-off limit for world {worldId}. Will no longer attempt to subscribe to this world.", ex);
                this.backoffCounts[worldId] = 0;
            }
            else
            {
                await Task.Delay((int)(BackoffSeconds * 1000), cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                subscriptionQueue.Enqueue(nextMessage);
            }
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
    }

    private async Task ReceiveMessages(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        if (!cancellationToken.IsCancellationRequested)
        {
            WebSocketReceiveResult result;
            var messageBytes = new List<byte>();

            do
            {
                try
                {
                    result = await this.Client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                }
                catch (TimeoutException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    this.pluginLog.Info("WebSocket connection closed.");
                    return;
                }

                messageBytes.AddRange(buffer.Take(result.Count));
            }
            while (!result.EndOfMessage);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                this.pluginLog.Info("WebSocket connection closed.");
                await this.Client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
            }
            else
            {
                var message = BsonSerializer.Deserialize<SubscriptionReceivedMessage>(messageBytes.ToArray());
                this.OnUniversalisEvent?.Invoke(message);
            }
        }
    }

    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                // Websocket clients can only be used once so we need to request a new one.
                if (this.Client.State == WebSocketState.Closed)
                {
                    this.client = this.websocketFactory.Invoke();
                }

                await this.Client.ConnectAsync(this.uri, cancellationToken);
                if (this.AutoResubscribe && this.subscriptions.Count != 0)
                {
                    this.pluginLog.Info("Resubscribing to worlds.");
                    var toSubscribe = this.subscriptions.ToHashSet();
                    this.subscriptions.Clear();
                    foreach (var subscription in toSubscribe)
                    {
                        this.SubscribeToChannel(subscription.Item1, subscription.Item2);
                    }
                }
                this.subscriptions.Clear();
            }

            this.pluginLog.Info("Connected to universalis websocket.");
        }
        catch (Exception ex)
        {
            this.pluginLog.Error(ex, "Failed to connect to universalis. Retrying in 5 seconds...");
            if (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5000, cancellationToken);
            }
        }
    }


}
