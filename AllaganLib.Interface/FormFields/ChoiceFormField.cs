using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.FormFields;

public abstract class ChoiceFormField<TValue, TConfiguration> : FormField<TValue, TConfiguration>
    where TValue : IComparable
    where TConfiguration : IConfigurable<TValue?>
{
    public ChoiceFormField(ImGuiService imGuiService)
        : base(imGuiService)
    {
    }

    public override TValue CurrentValue(TConfiguration configurable)
    {
        return configurable.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(TConfiguration configurable, TValue? newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public abstract Dictionary<TValue, string> Choices { get; }

    public virtual string GetFormattedChoice(TValue choice)
    {
        return this.Choices.SingleOrDefault(c => c.Key.Equals(choice)).Value;
    }

    public override bool DrawInput(TConfiguration configuration, int? inputSize = null)
    {
        var choices = this.Choices;
        var activeChoice = this.CurrentValue(configuration);
        var wasUpdated = false;

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
                        if (this.AutoSave)
                        {
                            this.UpdateFilterConfiguration(configuration, item.Key);
                        }

                        wasUpdated = true;
                    }
                }
            }
        }

        return wasUpdated;
    }
}