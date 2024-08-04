using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllaganLib.Shared.Interfaces;

/// <summary>
/// A interface for a background task queue.
/// </summary>
public interface IBackgroundTaskQueue
{
    /// <summary>
    /// Queue a work item to be processed.
    /// </summary>
    /// <param name="workItem">The task to perform.</param>
    /// <returns>A ValueTask that represents the asynchronous write operation.</returns>
    Task QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem);

    /// <summary>
    /// Asynchronously reads an item from the channel.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A ValueTask that represents the asynchronous read operation.</returns>
    Task<Func<CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// The number of items that are queued.
    /// </summary>
    int QueueCount { get; }
}