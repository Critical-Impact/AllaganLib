using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AllaganLib.Shared.Interfaces;
using AllaganLib.Shared.Services;
using AllaganLib.Universalis.Models;

using Dalamud.Plugin.Services;

using Lumina.Excel.Sheets;

using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

namespace AllaganLib.Universalis.Services;

public class UniversalisApiService : BackgroundService
{
    public delegate void PriceRetrievedDelegate(uint itemId, uint worldId, UniversalisPricing response);

    public event PriceRetrievedDelegate? PriceRetrieved;

    private readonly IPluginLog pluginLog;
    private readonly IFramework framework;
    private readonly IDataManager dataManager;
    private Dictionary<uint, string> worldNames = new();

    public HttpClient HttpClient { get; }

    public IBackgroundTaskQueue UniversalisQueue { get; }

    public uint QueueTime { get; } = 5;

    public uint MaxRetries { get; } = 3;

    public DateTime? LastFailure { get; private set; }

    public bool TooManyRequests { get; private set; }

    public int QueuedCount => this.queuedCount;

    public UniversalisApiService(
        IPluginLog pluginLog,
        HttpClient httpClient,
        IFramework framework,
        IDataManager dataManager)
    {
        this.pluginLog = pluginLog;
        this.framework = framework;
        this.dataManager = dataManager;
        this.HttpClient = httpClient;
        this.UniversalisQueue = new BackgroundTaskQueue(1);
        this.framework.Update += this.FrameworkOnUpdate;
    }

    private void FrameworkOnUpdate(IFramework framework)
    {
        foreach (var world in this.queueWorldItemIds)
        {
            if (world.Value.Item1 < DateTime.Now)
            {
                this.queueWorldItemIds.Remove(world.Key, out var fullList);
                this.queuedCount += fullList.Item2.Count;
                this.UniversalisQueue.QueueBackgroundWorkItemAsync(
                    token => this.RetrieveMarketBoardPrices(fullList.Item2, world.Key, token));
                break;
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this.BackgroundProcessing(stoppingToken);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem =
                await this.UniversalisQueue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                this.pluginLog.Error(
                    ex,
                    "Error occurred executing {WorkItem}.",
                    nameof(workItem));
            }
        }
    }

    public void QueuePriceCheck(uint itemId, uint worldId)
    {
        if (itemId == 0 || worldId == 0)
        {
            return;
        }

        this.queueWorldItemIds.TryAdd(worldId, (DateTime.Now.AddSeconds(this.QueueTime), []));
        this.queueWorldItemIds[worldId].Item2.Add(itemId);
        if (this.queueWorldItemIds[worldId].Item2.Count == 50)
        {
            this.queueWorldItemIds.Remove(worldId, out var fullList);
            this.queuedCount += fullList.Item2.Count;
            this.UniversalisQueue.QueueBackgroundWorkItemAsync(
                token => this.RetrieveMarketBoardPrices(fullList.Item2, worldId, token));
        }
    }

    private ConcurrentDictionary<uint, (DateTime, HashSet<uint>)> queueWorldItemIds = new();
    private int queuedCount;

    public async Task RetrieveMarketBoardPrices(
        IEnumerable<uint> itemIds,
        uint worldId,
        CancellationToken token,
        uint attempt = 0)
    {
        if (token.IsCancellationRequested)
        {
            return;
        }

        var itemIdList = itemIds.ToList();
        if (attempt == this.MaxRetries)
        {
            this.queuedCount -= itemIdList.Count;
            this.pluginLog.Error($"Maximum retries for universalis has been reached, cancelling.");
            return;
        }

        string worldName;
        if (!this.worldNames.ContainsKey(worldId))
        {
            var world = this.dataManager.GetExcelSheet<World>().GetRowOrDefault(worldId);
            if (world == null)
            {
                this.queuedCount -= itemIdList.Count;
                return;
            }

            this.worldNames[worldId] = world.Value.Name.ExtractText();
        }

        worldName = this.worldNames[worldId];

        var itemIdsString = string.Join(",", itemIdList.Select(c => c.ToString()).ToArray());
        this.pluginLog.Verbose($"Sending request for items {itemIdsString} to universalis API.");
        var url =
            $"https://universalis.app/api/v2/{worldName}/{itemIdsString}";
        try
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var response = await this.HttpClient.GetAsync(url, token);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                this.pluginLog.Warning("Too many requests to universalis, waiting a minute.");
                this.TooManyRequests = true;
                await Task.Delay(TimeSpan.FromMinutes(1), token);
                await this.RetrieveMarketBoardPrices(itemIdList, worldId, token, attempt + 1);
                return;
            }

            this.TooManyRequests = false;

            var value = await response.Content.ReadAsStringAsync(token);

            if (itemIdList.Count == 1)
            {
                UniversalisRequestSingle? apiListing = JsonConvert.DeserializeObject<UniversalisRequestSingle>(value);

                if (apiListing != null)
                {
                    var listing = UniversalisPricing.FromApi(
                        apiListing,
                        worldId);
                    await this.framework.RunOnFrameworkThread(
                        () =>
                            this.PriceRetrieved?.Invoke(apiListing.itemID, worldId, listing));
                }
                else
                {
                    this.pluginLog.Error("Failed to parse universalis json data, backing off 30 seconds.");
                    this.LastFailure = DateTime.Now;
                    await Task.Delay(TimeSpan.FromSeconds(30), token);
                }
            }
            else
            {
                UniversalisRequestMulti? multiRequest = JsonConvert.DeserializeObject<UniversalisRequestMulti>(value);
                if (multiRequest != null)
                {
                    foreach (var item in multiRequest.items)
                    {
                        var listing = UniversalisPricing.FromApi(
                            item.Value,
                            worldId);
                        await this.framework.RunOnFrameworkThread(
                            () =>
                                this.PriceRetrieved?.Invoke(item.Value.itemID, worldId, listing));
                    }
                }
                else
                {
                    this.pluginLog.Error(
                        "Failed to parse universalis multi request json data, backing off 30 seconds.");
                    this.LastFailure = DateTime.Now;
                    await Task.Delay(TimeSpan.FromSeconds(30), token);
                }
            }
        }
        catch (TaskCanceledException ex)
        {
        }
        catch (JsonReaderException readerException)
        {
            this.pluginLog.Error(readerException.ToString());
            this.pluginLog.Error("Failed to parse universalis data, backing off 30 seconds.");
            this.LastFailure = DateTime.Now;
            await Task.Delay(TimeSpan.FromSeconds(30), token);
        }
        catch (Exception ex)
        {
            this.pluginLog.Debug(ex.ToString());
        }

        this.queuedCount -= itemIdList.Count;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.framework.Update -= this.FrameworkOnUpdate;
        }
    }

    public sealed override void Dispose()
    {
        this.Dispose(true);
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
