using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AllaganLib.Shared.Interfaces;

namespace AllaganLib.Shared.Services;

/// <summary>
/// A background task queue for processing items in parallel.
/// </summary>
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> queue;

    public delegate BackgroundTaskQueue Factory(string queueName, int capacity = 10);

    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundTaskQueue"/> class.
    /// </summary>
    /// <param name="capacity">How many items should be processed at a time.</param>
    public BackgroundTaskQueue(BackgroundTaskCollector backgroundTaskCollector, string queueName, int capacity = 10)
    {
        this.QueueName = queueName;
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };
        this.queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
        backgroundTaskCollector.RegisterBackgroundTaskQueue(this);
    }

    /// <inheritdoc/>
    public async Task QueueBackgroundWorkItemAsync(
        Func<CancellationToken, Task> workItem)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await this.queue.Writer.WriteAsync(workItem);
    }

    /// <inheritdoc/>
    public async Task<Func<CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        var workItem = await this.queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }

    /// <inheritdoc/>
    public int QueueCount => this.queue.Reader.Count;

    /// <inheritdoc/>
    public string QueueName { get; }
}
