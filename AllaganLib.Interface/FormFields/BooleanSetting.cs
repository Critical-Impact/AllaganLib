using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using ImGuiNET;

namespace AllaganLib.Interface.FormFields;

public abstract class BooleanSetting<T> : FormField<bool, T>
    where T : IConfigurable<bool?>
{
    private readonly string[] Choices = new[] { "N/A", "Yes", "No" };

    protected BooleanSetting(ImGuiService imGuiService)
        : base(imGuiService)
    {
    }

    public override bool CurrentValue(T configurable)
    {
        return configurable.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(T configurable, bool newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public override void Draw(T configuration)
    {
        var currentValue = this.CurrentValue(configuration);
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

        if (ImGui.Checkbox("##" + this.Key + "Boolean", ref currentValue))
        {
            if (currentValue != this.CurrentValue(configuration))
            {
                this.UpdateFilterConfiguration(configuration, currentValue);
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
}