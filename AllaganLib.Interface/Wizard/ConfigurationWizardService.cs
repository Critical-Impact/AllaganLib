using System.Collections.Generic;
using System.Linq;

using AllaganLib.Interface.FormFields;

namespace AllaganLib.Interface.Wizard;

/// <inheritdoc />
public class ConfigurationWizardService<T> : IConfigurationWizardService<T>
    where T : class, IWizardConfiguration
{
    private readonly T configuration;
    private readonly List<IFeature<T>> availableFeatures;
    private readonly Dictionary<IFeature<T>, List<IFormField<T>>> versionedSettings = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationWizardService{T}"/> class.
    /// </summary>
    /// <param name="features">A list of all available features.</param>
    /// <param name="configuration">The configuration class to configure.</param>
    public ConfigurationWizardService(IEnumerable<IFeature<T>> features, T configuration)
    {
        this.configuration = configuration;
        this.availableFeatures = features.ToList();
        foreach (var feature in this.availableFeatures)
        {
            foreach (var setting in feature.RelatedSettings)
            {
                setting.Name = setting.WizardName;
                setting.HideReset = true;
                setting.ColourModified = false;
            }
        }
    }

    /// <inheritdoc/>
    public bool HasNewFeatures => this.GetNewFeatures().Count != 0;

    /// <inheritdoc/>
    public bool ShouldShowWizard => this.HasNewFeatures && this.configuration.ShowWizardNewFeatures;

    /// <inheritdoc/>
    public bool ConfiguredOnce => this.configuration.WizardVersionsSeen.Count != 0;

    /// <summary>
    /// Returns the settings that are applicable for this feature for this version.
    /// </summary>
    /// <param name="feature">The feature.</param>
    /// <returns>A list of applicable settings.</returns>
    public List<IFormField<T>> GetApplicableSettings(IFeature<T> feature)
    {
        if (!this.versionedSettings.TryGetValue(feature, out var value))
        {
            var relatedSettings = feature.RelatedSettings;
            value = relatedSettings.Where(c => !this.configuration.WizardVersionsSeen.Contains(c.Version)).ToList();
            this.versionedSettings[feature] = value;
        }

        return value;
    }

    /// <inheritdoc/>
    public List<IFeature<T>> GetFeatures()
    {
        return this.availableFeatures.ToList();
    }

    /// <inheritdoc/>
    public List<IFeature<T>> GetNewFeatures()
    {
        var versionsSeen = this.configuration.WizardVersionsSeen;
        return this.availableFeatures.Where(
            c => !c.RelatedSettings.Select(d => d.Version).Distinct().All(v => versionsSeen.Contains(v))).ToList();
    }

    /// <inheritdoc/>
    public void MarkFeaturesSeen()
    {
        var seenVersions = this.availableFeatures.SelectMany(c => c.RelatedSettings).Select(c => c.Version).Distinct();
        foreach (var version in seenVersions)
        {
            this.configuration.MarkWizardVersionSeen(version);
        }
    }

    /// <inheritdoc/>
    public void ClearFeaturesSeen()
    {
        this.configuration.WizardVersionsSeen = new HashSet<string>();
    }
}