using System.Collections.Generic;

using AllaganLib.Interface.FormFields;

namespace AllaganLib.Interface.Wizard;

/// <summary>
/// An interface for implementing a configuration wizard service.
/// </summary>
/// <typeparam name="T">A configuration class that implements IWizardConfiguration.</typeparam>
public interface IConfigurationWizardService<T>
    where T : IWizardConfiguration
{
    /// <summary>
    /// Gets a value indicating whether there are new features for the user to configure.
    /// </summary>
    public bool HasNewFeatures { get; }

    /// <summary>
    /// Gets a value indicating whether should the wizard be shown.
    /// </summary>
    public bool ShouldShowWizard { get; }

    /// <summary>
    /// Gets a value indicating whether the wizard been run at least once.
    /// </summary>
    public bool ConfiguredOnce { get; }

    /// <summary>
    /// Gets all available features.
    /// </summary>
    /// <returns>A list of all available features.</returns>
    public List<IFeature<T>> GetFeatures();

    /// <summary>
    /// Get any new features the user has not yet configured.
    /// </summary>
    /// <returns>A list of any features the user has not configured.</returns>
    public List<IFeature<T>> GetNewFeatures();

    /// <summary>
    /// Returns the settings that are applicable for this feature for this version.
    /// </summary>
    /// <param name="feature">The feature.</param>
    /// <returns>A list of applicable settings to the feature.</returns>
    List<IFormField<T>> GetApplicableSettings(IFeature<T> feature);

    /// <summary>
    /// Mark all available features as seen by the user.
    /// </summary>
    public void MarkFeaturesSeen();

    /// <summary>
    /// Clear the features the user has seen so they see them again.
    /// </summary>
    public void ClearFeaturesSeen();
}