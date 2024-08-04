using System;
using System.Numerics;
using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using ImGuiNET;

namespace AllaganLib.Interface.FormFields;

public abstract class ColorSetting<T> : FormField<Vector4, T>
    where T : IConfigurable<Vector4?>
{
    public ColorSetting(ImGuiService imGuiService)
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

    public override void Draw(T configuration)
    {
        var value = this.CurrentValue(configuration);

        if (ImGui.ColorEdit4(
                "##" + this.Key + "Color",
                ref value,
                ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel))
        {
            this.UpdateFilterConfiguration(configuration, value);
        }

        ImGui.SameLine();
        if (this.HasValueSet(configuration) && value.W == 0)
        {
            ImGui.SameLine();
            ImGui.TextColored(ImGuiColors.DalamudRed, "The alpha is currently set to 0, this will be invisible.");
        }

        ImGui.SameLine();
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
}