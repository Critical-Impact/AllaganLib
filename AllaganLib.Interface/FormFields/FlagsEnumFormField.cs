using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.Interface.Services;
using AllaganLib.Shared.Extensions;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Common.Math;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.FormFields;


public abstract class FlagsEnumFormField<T, TS> : FormField<T, TS>
    where T : struct, Enum
    where TS : IConfigurable<T>
{
    private string? comboLabel;

    public FlagsEnumFormField(ImGuiService imGuiService)
        : base(imGuiService)
    {
    }

    public abstract T AddFlag(T currentFlags, T newFlag);

    public abstract T RemoveFlag(T currentFlags, T existingFlag);

    public abstract bool FlagEmpty(T flag);

    public abstract string GetComboLabel(TS configuration);

    public override T CurrentValue(TS configurable)
    {
        return configurable.Get(this.Key);
    }

    public override void UpdateFilterConfiguration(TS configurable, T newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public override bool Draw(TS configuration, int? labelSize = null, int? inputSize = null)
    {
        var wasUpdated = this.DrawSearchBox(configuration);
        var wasUpdated2 = this.DrawResults(configuration);
        return wasUpdated || wasUpdated2;
    }

    public override bool DrawInput(TS configuration, int? inputSize = null)
    {
        var choices = this.GetChoices(configuration);
        var selectedChoices = this.CurrentValue(configuration);
        var currentSearchCategory = this.comboLabel ??= this.GetComboLabel(configuration);
        var wasUpdated = false;

        ImGui.SetNextItemWidth(inputSize ?? this.InputSize);
        using (var combo = ImRaii.Combo("##" + this.Key + "Combo", currentSearchCategory, ImGuiComboFlags.HeightLarge))
        {
            if (combo.Success)
            {
                var searchString = this.SearchString;
                ImGui.InputText("Start typing to search..##ItemSearch", ref searchString, 50);
                if (this._searchString != searchString)
                {
                    this.SearchString = searchString;
                }

                var activeChoices = this.GetActiveChoices(configuration);
                ImGui.SameLine();
                if (ImGui.Button("Add All"))
                {
                    foreach (var item in activeChoices)
                    {
                        if (!selectedChoices.HasFlag(item.Key))
                        {
                            selectedChoices = this.AddFlag(selectedChoices, item.Key);
                            wasUpdated = true;
                        }
                    }

                    this.cachedChoices = null;
                    this.comboLabel = null;
                    this.UpdateFilterConfiguration(configuration, selectedChoices);
                }

                ImGui.Separator();
                using (var child = ImRaii.Child("searchBox", new Vector2(0, 250)))
                {
                    if (child)
                    {
                        foreach (var item in activeChoices)
                        {
                            if (item.Value == string.Empty)
                            {
                                continue;
                            }

                            var isSelected = selectedChoices.HasFlag(item.Key);
                            if (!this.FlagEmpty(selectedChoices) && this.FlagEmpty(item.Key))
                            {
                                isSelected = false;
                            }

                            if (ImGui.Selectable(
                                    item.Value.Replace("\u0002\u001F\u0001\u0003", "-"),
                                    isSelected))
                            {
                                if (isSelected)
                                {
                                    wasUpdated = true;
                                    selectedChoices = this.RemoveFlag(selectedChoices, item.Key);
                                    if (this.AutoSave)
                                    {
                                        this.UpdateFilterConfiguration(configuration, selectedChoices);
                                    }

                                    this.comboLabel = null;
                                    this.cachedChoices = null;
                                }
                                else
                                {
                                    wasUpdated = true;
                                    selectedChoices = this.AddFlag(selectedChoices, item.Key);
                                    if (this.AutoSave)
                                    {
                                        this.UpdateFilterConfiguration(configuration, selectedChoices);
                                    }

                                    this.comboLabel = null;
                                    this.cachedChoices = null;
                                }
                            }
                        }
                    }
                }
            }
        }

        return wasUpdated;
    }

    public virtual bool DrawSearchBox(TS configuration, int? labelSize = null, int? inputSize = null)
    {
        this.DrawLabel(configuration, labelSize);
        ImGui.SameLine();
        var wasUpdated = this.DrawInput(configuration, inputSize);
        ImGui.SameLine();
        this.DrawHelp(configuration);
        return wasUpdated;
    }

    public virtual bool DrawResults(TS configuration, int? labelSize = null, int? inputSize = null)
    {
        var choices = this.GetChoices(configuration);
        var selectedChoices = this.CurrentValue(configuration);
        var wasUpdated = false;

        var enumValues = Enum.GetValues<T>().Where(c => selectedChoices.HasFlag(c)).ToList();

        for (var index = 0; index < enumValues.Count; index++)
        {
            var item = enumValues[index];

            var actualItem = choices.GetValueOrDefault(item);
            var selectedChoicesCount = enumValues.Count;
            if (actualItem != null)
            {
                var itemSearchCategoryName = actualItem
                    .Replace("\u0002\u001F\u0001\u0003", "-");
                if (ImGui.Button(itemSearchCategoryName + " X" + "##" + this.Key + index))
                {
                    if (selectedChoices.HasFlag(item))
                    {
                        selectedChoices = this.RemoveFlag(selectedChoices, item);
                        if (this.AutoSave)
                        {
                            this.UpdateFilterConfiguration(configuration, selectedChoices);
                        }

                        wasUpdated = true;
                    }
                }
            }

            if (index != selectedChoicesCount - 1 &&
                (index % 4 != 0 || index == 0))
            {
                ImGui.SameLine();
            }
        }

        return wasUpdated;
    }

    public abstract Dictionary<T, string> GetChoices(TS configuration);

    private Dictionary<T, string>? cachedChoices;

    public virtual Dictionary<T, string> GetActiveChoices(TS configuration)
    {
        if (this.cachedChoices != null)
        {
            return this.cachedChoices;
        }

        var choices = this.GetChoices(configuration);
        IEnumerable<KeyValuePair<T, string>> filteredChoices;
        var searchString = this.SearchString.ToParseable();
        if (this.HideAlreadyPicked)
        {
            var currentChoices = this.CurrentValue(configuration);
            filteredChoices = choices.Where(
                c => this.FilterSearch(c.Key, c.Value, searchString) && !currentChoices.HasFlag(c.Key));
        }
        else
        {
            filteredChoices = choices.Where(c => this.FilterSearch(c.Key, c.Value, searchString));
        }

        if (this.ResultLimit != null)
        {
            filteredChoices = filteredChoices.Take(this.ResultLimit.Value);
        }

        this.cachedChoices = filteredChoices.ToDictionary(c => c.Key, c => c.Value);
        ;
        return this.cachedChoices;
    }

    public abstract bool HideAlreadyPicked { get; set; }

    public virtual int? ResultLimit { get; } = null;

    private string _searchString = "";

    public virtual bool FilterSearch(T itemId, string itemName, string searchString)
    {
        if (searchString == "")
        {
            return true;
        }

        return itemName.ToParseable().PassesFilter(searchString);
    }

    public string SearchString
    {
        get => this._searchString;
        set
        {
            this._searchString = value;
            this.cachedChoices = null;
        }
    }

    public override bool HasValueSet(TS configuration)
    {
        var currentValue = this.CurrentValue(configuration);
        return !currentValue.Equals(this.DefaultValue);
    }

    public override void Reset(TS configuration)
    {
        base.Reset(configuration);
    }

    public override FormFieldType FieldType => FormFieldType.FlagsEnum;
}
