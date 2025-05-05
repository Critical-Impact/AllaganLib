namespace AllaganLib.Universalis.Models;

/// <summary>
/// A class that provides the user agent to universalis.
/// </summary>
public class UniversalisUserAgent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalisUserAgent"/> class.
    /// </summary>
    /// <param name="pluginName">The internal name of the plugin.</param>
    /// <param name="pluginVersion">The version of the plugin.</param>
    public UniversalisUserAgent(string pluginName, string pluginVersion)
    {
        this.PluginName = pluginName;
        this.PluginVersion = pluginVersion;
    }

    /// <summary>
    /// Gets the internal name of the plugin.
    /// </summary>
    public string PluginName { get; }

    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    public string PluginVersion { get; }
}
