using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace AllaganLib.Interface.FormFields;

public abstract class ChoiceFormField<T, TS> : FormField<T, TS>
    where T : IComparable
    where TS : IConfigurable<T?>
{
    public ChoiceFormField(ImGuiService imGuiService)
        : base(imGuiService)
    {
    }

    public override T CurrentValue(TS configurable)
    {
        return configurable.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(TS configurable, T? newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public abstract Dictionary<T, string> Choices { get; }

    public virtual string GetFormattedChoice(T choice)
    {
        return this.Choices.SingleOrDefault(c => c.Key.Equals(choice)).Value;
    }

    public override void DrawInput(TS configuration, int? inputSize = null)
    {
        var choices = this.Choices;
        var activeChoice = this.CurrentValue(configuration);

        var currentSearchCategory = this.GetFormattedChoice(activeChoice);
        ImGui.SetNextItemWidth(inputSize ?? this.InputSize);
        using (var combo = ImRaii.Combo("##" + this.Key + "Combo", currentSearchCategory))
        {
            if (combo.Success)
            {
                foreach (var item in choices)
                {
                    var text = item.Value.Replace("\u0002\u001F\u0001\u0003", "-");
                    if (text == "")
                    {
                        continue;
                    }

                    if (ImGui.Selectable(text, currentSearchCategory == text))
                    {
                        this.UpdateFilterConfiguration(configuration, item.Key);
                    }
                }
            }
        }
    }
}