using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AllaganLib.Monitors.Interfaces;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AllaganLib.Monitors.Services;

public unsafe class AchievementMonitorService :
    IAchievementMonitorService,
    IHostedService,
    IDisposable
{
    private readonly IFramework framework;
    private readonly ExcelSheet<Achievement> achievementSheet;
    private readonly ILogger<AchievementMonitorService> logger;
    private readonly IAchievementMonitorConfiguration configuration;

    private HashSet<uint> completedAchievementIds = new();
    private DateTime lastPollTime;

    public AchievementMonitorService(
        IFramework framework,
        ExcelSheet<Achievement> achievementSheet,
        IAchievementMonitorConfiguration configuration,
        ILogger<AchievementMonitorService> logger)
    {
        this.framework = framework;
        this.achievementSheet = achievementSheet;
        this.configuration = configuration;
        this.logger = logger;
    }

    public bool IsCompleted(uint achievementId)
    {
        return this.completedAchievementIds.Contains(achievementId);
    }

    /// <inheritdoc/>
    public bool IsLoaded { get; private set; }

    /// <inheritdoc/>
    public IAchievementMonitorConfiguration Configuration => this.configuration;

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.framework.Update += this.FrameworkOnUpdate;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.framework.Update -= this.FrameworkOnUpdate;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.framework.Update -= this.FrameworkOnUpdate;
    }

    /// <inheritdoc/>
    public List<uint> GetCompletedAchievementIds()
    {
        return this.completedAchievementIds.ToList();
    }

    /// <inheritdoc/>
    public List<RowRef<Achievement>> GetCompletedAchievements()
    {
        return this.completedAchievementIds
            .Select(id => this.achievementSheet.GetRowOrDefault(id))
            .Where(row => row != null)
            .Select(row => new RowRef<Achievement>(this.achievementSheet.Module, row!.Value.RowId))
            .ToList();
    }

    private void FrameworkOnUpdate(IFramework _)
    {
        try
        {
            var instance = FFXIVClientStructs.FFXIV.Client.Game.UI.Achievement.Instance();
            if (instance == null)
            {
                this.IsLoaded = false;
                return;
            }

            if (!instance->IsLoaded())
            {
                this.IsLoaded = false;
                return;
            }

            this.IsLoaded = true;

            if ((DateTime.UtcNow - this.lastPollTime).TotalSeconds < this.configuration.PollIntervalSeconds)
            {
                return;
            }

            this.lastPollTime = DateTime.UtcNow;
            this.RefreshCompletedAchievements(instance);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to update achievement state");
        }
    }

    private void RefreshCompletedAchievements(FFXIVClientStructs.FFXIV.Client.Game.UI.Achievement* achievement)
    {
        var newCompleted = new HashSet<uint>();

        foreach (var row in this.achievementSheet)
        {
            if (achievement->IsComplete((int)row.RowId))
            {
                newCompleted.Add(row.RowId);
            }
        }

        if (newCompleted.SetEquals(this.completedAchievementIds))
        {
            return;
        }

        var newlyCompleted = newCompleted.Except(this.completedAchievementIds).ToList();

        if (newlyCompleted.Count > 0)
        {
            this.logger.LogTrace(
                "Detected {Count} newly completed achievements",
                newlyCompleted.Count);
        }

        this.completedAchievementIds = newCompleted;
    }
}