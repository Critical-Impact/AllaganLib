using System.Collections.Generic;
using System.Linq;
using AllaganLib.Interface.Services;
using AllaganLib.Shared.Extensions;
using Dalamud.Interface.Colors;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using Dalamud.Interface.Utility.Raii;

namespace AllaganLib.Interface.FormFields;

public abstract class MultipleChoiceSetting<T, TS> : FormField<List<T>, TS>
    where T : notnull
    where TS : IConfigurable<List<T>?>
{
    public MultipleChoiceSetting(ImGuiService imGuiService)
        : base(imGuiService)
    {
    }
    
    public override List<T> CurrentValue(TS configurable)
    {
        return configurable.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(TS configurable, List<T>? newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public override void Draw(TS configuration)
    {
        var currentX = ImGui.GetCursorPosX();
        currentX += ImGui.GetFontSize() + (ImGui.GetStyle().FramePadding.X * 2.0f) +
                    ImGui.GetStyle().ItemInnerSpacing.X;
        this.DrawSearchBox(configuration, currentX);
        this.DrawResults(configuration, currentX);
    }

    public virtual void DrawSearchBox(TS configuration, float currentX)
    {
        ImGui.SetNextItemWidth(this.LabelSize);
        if (this.ColourModified && this.HasValueSet(configuration))
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            ImGui.LabelText("##" + this.Key + "Label", this.Name);
            ImGui.PopStyleColor();
        }
        else
        {
            ImGui.LabelText("##" + this.Key + "Label", this.Name);
        }

        var choices = this.GetChoices(configuration);
        var selectedChoices = this.CurrentValue(configuration);
        var currentSearchCategory = "";
        ImGui.SetNextItemWidth(this.InputSize);
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
                        if (!selectedChoices.Contains(item.Key))
                        {
                            selectedChoices.Add(item.Key);
                        }
                    }

                    this._cachedChoices = null;
                    this.UpdateFilterConfiguration(configuration, selectedChoices);
                }

                ImGui.Separator();
                using (ImRaii.Child("searchBox", new Vector2(0, 250)))
                {
                    foreach (var item in activeChoices)
                    {
                        if (item.Value == "")
                        {
                            continue;
                        }

                        if (ImGui.Selectable(
                                item.Value.Replace("\u0002\u001F\u0001\u0003", "-"),
                                currentSearchCategory == item.Value))
                        {
                            if (!selectedChoices.Contains(item.Key))
                            {
                                selectedChoices.Add(item.Key);
                                this.UpdateFilterConfiguration(configuration, selectedChoices);
                                this._cachedChoices = null;
                            }
                        }
                    }
                }
            }
        }

        ImGui.SameLine();
        this.ImGuiService.HelpMarker(this.HelpText, this.Image, this.ImageSize);
        if (!this.HideReset && this.HasValueSet(configuration))
        {
            ImGui.SameLine();
            if (ImGui.Button("Reset##" + this.Key + "Reset"))
            {
                this.Reset(configuration);
            }
        }
    }

    public virtual void DrawResults(TS configuration, float currentX)
    {
        var choices = this.GetChoices(configuration);
        var selectedChoices = this.CurrentValue(configuration);

        for (var index = 0; index < selectedChoices.Count; index++)
        {
            var item = selectedChoices[index];
            var actualItem = choices.ContainsKey(item) ? choices[item] : null;
            var selectedChoicesCount = selectedChoices.Count;
            if (actualItem != null)
            {
                var itemSearchCategoryName = actualItem
                    .Replace("\u0002\u001F\u0001\u0003", "-");
                if (ImGui.Button(itemSearchCategoryName + " X" + "##" + this.Key + index))
                {
                    if (selectedChoices.Contains(item))
                    {
                        selectedChoices.Remove(item);
                        this.UpdateFilterConfiguration(configuration, selectedChoices);
                    }
                }
            }

            if (index != selectedChoicesCount - 1 &&
                (index % 4 != 0 || index == 0))
            {
                ImGui.SameLine();
            }
        }
    }

    public abstract Dictionary<T, string> GetChoices(TS configuration);

    private Dictionary<T, string>? _cachedChoices;

    public virtual Dictionary<T, string> GetActiveChoices(TS configuration)
    {
        if (this._cachedChoices != null)
        {
            return this._cachedChoices;
        }

        var choices = this.GetChoices(configuration);
        IEnumerable<KeyValuePair<T, string>> filteredChoices;
        var searchString = this.SearchString.ToParseable();
        if (this.HideAlreadyPicked)
        {
            var currentChoices = this.CurrentValue(configuration);
            filteredChoices = choices.Where(
                c => this.FilterSearch(c.Key, c.Value, searchString) && !currentChoices.Contains(c.Key));
        }
        else
        {
            filteredChoices = choices.Where(c => this.FilterSearch(c.Key, c.Value, searchString));
        }

        if (this.ResultLimit != null)
        {
            filteredChoices = filteredChoices.Take(this.ResultLimit.Value);
        }

        this._cachedChoices = filteredChoices.ToDictionary(c => c.Key, c => c.Value);
        ;
        return this._cachedChoices;
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
            this._cachedChoices = null;
        }
    }

    public override bool HasValueSet(TS configuration)
    {
        var currentValue = this.CurrentValue(configuration).Distinct().ToHashSet();
        return !currentValue.SetEquals(this.DefaultValue.Distinct().ToHashSet());
    }

    public override void Reset(TS configuration)
    {
        base.Reset(configuration);
    }
}