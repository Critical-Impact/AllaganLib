using System;
using System.Numerics;
using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using ImGuiNET;

namespace AllaganLib.Interface.FormFields;

public abstract class ColorFormField<T> : FormField<Vector4, T>
    where T : IConfigurable<Vector4?>
{
    public ColorFormField(ImGuiService imGuiService)
        : base(imGuiService)
    {
    }

    public override Vector4 CurrentValue(T configuration)
    {
        return configuration.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(T configuration, Vector4 newValue)
    {
        configuration.Set(this.Key, newValue);
    }

    public override bool DrawInput(T configuration, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration);
        var wasUpdated = false;

        if (ImGui.ColorEdit4(
                "##" + this.Key + "Color",
                ref value,
                ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel))
        {
            if (this.AutoSave)
            {
                this.UpdateFilterConfiguration(configuration, value);
            }

            wasUpdated = true;
        }

        return wasUpdated;
    }

    public override bool Draw(T configuration, int? labelSize = null, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration);

        var result = this.DrawInput(configuration, inputSize);
        ImGui.SameLine();
        if (this.HasValueSet(configuration) && value.W == 0)
        {
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.DalamudRed, "The alpha is currently set to 0, this will be invisible.");
        }

        ImGui.SameLine();
        this.DrawLabel(configuration, labelSize);

        ImGui.SameLine();
        this.DrawHelp(configuration);

        return result;
    }
}