using System.Collections.Generic;

using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.Monitors.Interfaces;

/// <summary>
/// Tracks when a new item shows up in a character's primary inventory.
/// </summary>
public interface IAchievementMonitorService
{
    /// <summary>
    /// Get a list of completed achievement ids for the currently logged in character.
    /// </summary>
    /// <returns>A list of completed achievement ids.</returns>
    public List<uint> GetCompletedAchievementIds();

    /// <summary>
    /// Get a list of completed achievements as their lumina rows.
    /// </summary>
    /// <returns>A list of completed achievements as their lumina rows.</returns>
    public List<RowRef<Achievement>> GetCompletedAchievements();

    /// <summary>
    /// Returns if the achievement is completed.
    /// </summary>
    /// <param name="achievementId">The id of the achievement row from the achievement sheet.</param>
    /// <returns>If the achievement is completed.</returns>
    public bool IsCompleted(uint achievementId);

    /// <summary>
    /// Are the achievements currently loaded?
    /// </summary>
    public bool IsLoaded { get; }

    /// <summary>
    /// The configuration of the achievement monitor.
    /// </summary>
    public IAchievementMonitorConfiguration Configuration { get; }
}