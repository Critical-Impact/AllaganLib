namespace AllaganLib.Monitors.Interfaces;

public interface IAchievementMonitorConfiguration
{
    /// <summary>
    /// How often achievements should be polled.
    /// </summary>
    public int PollIntervalSeconds { get; set; }
}