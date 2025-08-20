namespace AllaganLib.Monitors.Interfaces;

/// <summary>
/// Allows for the acquisition monitor to be configured.
/// </summary>
public interface IAcquisitionMonitorConfiguration
{
    /// <summary>
    /// Gets or sets how long the acquisition tracker will wait before considering the user to be in another state.
    /// </summary>
    public int PersistStateTime { get; set; }

    /// <summary>
    /// Gets or sets how long the acquisition tracker should wait before starting to emit events after a character logins.
    /// </summary>
    public int LoginDelay { get; set; }
}