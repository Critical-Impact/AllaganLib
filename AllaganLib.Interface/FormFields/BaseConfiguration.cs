using System.Collections.Generic;
using System.ComponentModel;

using AllaganLib.Interface.Wizard;
using Dalamud.Bindings.ImGui;
using Newtonsoft.Json;

namespace AllaganLib.Interface.FormFields;

public class BaseConfiguration : IConfigurable<int?>, IConfigurable<uint?>, IConfigurable<string?>, IConfigurable<bool?>, IConfigurable<ImGuiKey?>, IWizardConfiguration
{
    private bool isDirty;
    private bool showWizardNewFeatures;
    private HashSet<string>? wizardVersionsSeen;

    public Dictionary<string, int> IntegerSettings { get; set; } = [];

    private Dictionary<string, uint> UnsignedIntegerSettings { get; set; } = [];

    private Dictionary<string, bool> BooleanSettings { get; set; } = [];

    private Dictionary<string, string> StringSettings { get; set; } = [];

    private Dictionary<string, ImGuiKey> ImGuiKeySettings { get; set; } = [];

    public int Version { get; set; }

    public void Set(string key, bool? newValue)
    {
        if (newValue == null)
        {
            this.BooleanSettings.Remove(key);
        }
        else
        {
            this.BooleanSettings[key] = newValue.Value;
        }

        this.IsDirty = true;
    }

    public void Set(string key, string? newValue)
    {
        if (newValue == null)
        {
            this.StringSettings.Remove(key);
        }
        else
        {
            this.StringSettings[key] = newValue;
        }

        this.IsDirty = true;
    }

    public void Set(string key, uint? newValue)
    {
        if (newValue == null)
        {
            this.UnsignedIntegerSettings.Remove(key);
        }
        else
        {
            this.UnsignedIntegerSettings[key] = newValue.Value;
        }

        this.IsDirty = true;
    }

    public void Set(string key, int? newValue)
    {
        if (newValue == null)
        {
            this.IntegerSettings.Remove(key);
        }
        else
        {
            this.IntegerSettings[key] = newValue.Value;
        }

        this.IsDirty = true;
    }

    int? IConfigurable<int?>.Get(string key)
    {
        return this.IntegerSettings.TryGetValue(key, out var value) ? value : null;
    }

    public void Set(string key, ImGuiKey? newValue)
    {
        if (newValue == null)
        {
            this.ImGuiKeySettings.Remove(key);
        }
        else
        {
            this.ImGuiKeySettings[key] = newValue.Value;
        }

        this.IsDirty = true;
    }

    uint? IConfigurable<uint?>.Get(string key)
    {
        return this.UnsignedIntegerSettings.TryGetValue(key, out var value) ? value : null;
    }

    string? IConfigurable<string?>.Get(string key)
    {
        return this.StringSettings.TryGetValue(key, out var value) ? value : null;
    }

    bool? IConfigurable<bool?>.Get(string key)
    {
        return this.BooleanSettings.TryGetValue(key, out var value) ? value : null;
    }

    public ImGuiKey? Get(string key)
    {
        return this.ImGuiKeySettings.GetValueOrDefault(key);
    }

    [JsonIgnore]
    public bool IsDirty
    {
        get => this.isDirty;
        set => this.isDirty = value;
    }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
    [DefaultValue(true)]
    public bool ShowWizardNewFeatures
    {
        get => this.showWizardNewFeatures;
        set
        {
            this.showWizardNewFeatures = value;
            this.isDirty = true;
        }
    }

    public HashSet<string> WizardVersionsSeen
    {
        get => this.wizardVersionsSeen ??= [];
        set
        {
            this.wizardVersionsSeen = value;
            this.IsDirty = true;
        }
    }

    public void MarkWizardVersionSeen(string versionNumber)
    {
        this.WizardVersionsSeen.Add(versionNumber);
        this.IsDirty = true;
    }
}