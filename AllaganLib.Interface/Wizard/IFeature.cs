using System.Collections.Generic;

using AllaganLib.Interface.FormFields;

namespace AllaganLib.Interface.Wizard;

/// <summary>
/// An interface that provides a set of settings that should be configured together.
/// </summary>
/// <typeparam name="T">A configuration class that implements IWizardConfiguration.</typeparam>
public interface IFeature<T>
{
    /// <summary>
    /// Gets a list of the settings that this feature has.
    /// </summary>
    public List<IFormField<T>> RelatedSettings { get; }

    /// <summary>
    /// Gets the name of the feature.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a description of the feature.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// A method that gets called when the feature has been configured.
    /// </summary>
    public void OnFinish();
}