using System.Collections.Generic;

namespace AllaganLib.Interface.Wizard;

/// <summary>
/// An interface that allows a configuration class to track if a user has configured a plugin.
/// </summary>
public interface IWizardConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether or not the user want to see new features.
    /// </summary>
    public bool ShowWizardNewFeatures { get; set; }

    /// <summary>
    /// Gets or sets a hashset of all the versions a user has seen.
    /// </summary>
    public HashSet<string> WizardVersionsSeen { get; set; }

    /// <summary>
    /// Returns a boolean indicating if they have seen the features from a particular version.
    /// </summary>
    /// <param name="versionNumber">The version number to check</param>
    /// <returns>A boolean indicating if they have seen the features from a particular version.</returns>
    public bool SeenWizardVersion(string versionNumber)
    {
        return this.WizardVersionsSeen.Contains(versionNumber);
    }

    /// <summary>
    /// Mark that a particular verion's features have been seen by the user.
    /// </summary>
    /// <param name="versionNumber">The version number.</param>
    public void MarkWizardVersionSeen(string versionNumber);
}