using System.Collections.Generic;
using AllaganLib.Shared.Interfaces;

namespace AllaganLib.Shared.Services;

public class BackgroundTaskCollector
{
    public List<IBackgroundTaskQueue> BackgroundTaskQueues { get; }

    public BackgroundTaskCollector()
    {
        this.BackgroundTaskQueues = new();
    }

    public void RegisterBackgroundTaskQueue(IBackgroundTaskQueue taskQueue)
    {
        this.BackgroundTaskQueues.Add(taskQueue);
    }
}