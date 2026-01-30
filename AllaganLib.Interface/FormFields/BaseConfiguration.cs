using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AllaganLib.Interface.Converters;
using AllaganLib.Interface.Wizard;
using Dalamud.Bindings.ImGui;
using Newtonsoft.Json;

namespace AllaganLib.Interface.FormFields;

public class BaseConfiguration : IConfigurable<int?>, IConfigurable<uint?>, IConfigurable<string?>, IConfigurable<bool?>, IConfigurable<ImGuiKey?>,  IConfigurable<Enum?>, IWizardConfiguration, INotifyPropertyChanged
{
    private bool isDirty;
    private bool showWizardNewFeatures;
    private HashSet<string>? wizardVersionsSeen;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Dictionary<string, int> IntegerSettings { get; set; } = [];

    public Dictionary<string, uint> UnsignedIntegerSettings { get; set; } = [];

    public Dictionary<string, bool> BooleanSettings { get; set; } = [];

    public Dictionary<string, string> StringSettings { get; set; } = [];

    public Dictionary<string, ImGuiKey> ImGuiKeySettings { get; set; } = [];

    [JsonConverter(typeof(EnumDictionaryConverter))]
    public Dictionary<string, Enum> EnumSettings { get; set; } = [];

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
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.BooleanSettings)));
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
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.StringSettings)));
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
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.UnsignedIntegerSettings)));
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
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IntegerSettings)));
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
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ImGuiKeySettings)));
        this.IsDirty = true;
    }

    public void Set(string key, Enum? newValue)
    {
        if (newValue == null)
        {
            this.EnumSettings.Remove(key);
        }
        else
        {
            this.EnumSettings[key] = newValue;
        }
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.EnumSettings)));
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

    Enum? IConfigurable<Enum?>.Get(string key)
    {
        return this.EnumSettings.GetValueOrDefault(key);
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
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WizardVersionsSeen)));
            this.isDirty = true;
        }
    }

    public HashSet<string> WizardVersionsSeen
    {
        get => this.wizardVersionsSeen ??= [];
        set
        {
            this.wizardVersionsSeen = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WizardVersionsSeen)));
            this.IsDirty = true;
        }
    }

    public void MarkWizardVersionSeen(string versionNumber)
    {
        this.WizardVersionsSeen.Add(versionNumber);
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WizardVersionsSeen)));
        this.IsDirty = true;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}