// <copyright file="NamedBackgroundTaskQueue.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AllaganLib.Shared.Interfaces;

namespace AllaganLib.Shared.Services;

public class NamedBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<NamedTask> queue;
    private readonly ConcurrentDictionary<string, NamedTask> taskMap = new();

    public delegate NamedBackgroundTaskQueue Factory(string queueName, int capacity = 10);

    public NamedBackgroundTaskQueue(string queueName, int capacity = 10)
    {
        this.QueueName = queueName;
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };
        this.queue = Channel.CreateBounded<NamedTask>(options);
    }

    public string QueueName { get; }

    public int QueueCount => this.queue.Reader.Count;

    private record NamedTask(
        string Name,
        Func<CancellationToken, Task> WorkItem,
        CancellationTokenSource CancellationSource);

    /// <summary>
    /// Queue a new task. If another task with the same name exists, it will be cancelled/replaced.
    /// </summary>
    public async Task QueueBackgroundWorkItemAsync(
        string name,
        Func<CancellationToken, Task> workItem)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Task name is required", nameof(name));
        }

        if (this.taskMap.TryGetValue(name, out var existing))
        {
            // Cancel the old one
            await existing.CancellationSource.CancelAsync();

            // Remove from map so we can replace
            this.taskMap.TryRemove(name, out _);
        }

        var cts = new CancellationTokenSource();
        var task = new NamedTask(name, workItem, cts);
        this.taskMap[name] = task;

        // Push to channel
        await this.queue.Writer.WaitToWriteAsync(CancellationToken.None);
        await this.queue.Writer.WriteAsync(task, CancellationToken.None);
    }

    /// <summary>
    /// Dequeue the next task for execution.
    /// </summary>
    public async Task<Func<CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        var namedTask = await this.queue.Reader.ReadAsync(cancellationToken);

        if (namedTask.CancellationSource.IsCancellationRequested)
        {
            return _ => Task.CompletedTask;
        }

        return ct => namedTask.WorkItem(CancellationTokenSource.CreateLinkedTokenSource(ct, namedTask.CancellationSource.Token).Token);
    }
}