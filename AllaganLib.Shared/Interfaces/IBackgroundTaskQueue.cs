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
    /// The number of items that are queued.
    /// </summary>
    int QueueCount { get; }

    /// <summary>
    /// The name of the queue.
    /// </summary>
    string QueueName { get; }
}