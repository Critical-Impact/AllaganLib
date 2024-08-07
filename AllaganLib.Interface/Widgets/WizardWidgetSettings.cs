namespace AllaganLib.Interface.Widgets;

/// <summary>
/// Provides the configuration for the wizard widget.
/// </summary>
public struct WizardWidgetSettings
{
    /// <summary>
    /// Gets the verbose name of your plugin.
    /// </summary>
    public string PluginName { get; init; }

    /// <summary>
    /// Gets the path to the logo of your plugin.
    /// </summary>
    public string LogoPath { get; init; }
}