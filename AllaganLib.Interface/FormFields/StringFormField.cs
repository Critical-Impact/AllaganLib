using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using ImGuiNET;

namespace AllaganLib.Interface.FormFields;

public abstract class StringFormField<T> : FormField<string, T>
    where T : IConfigurable<string?>
{
    public StringFormField(ImGuiService imGuiService)
        : base(imGuiService)
    {
    }

    public override string CurrentValue(T configurable)
    {
        return configurable.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(T configurable, string? newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public override bool HasValueSet(T configuration)
    {
        return this.CurrentValue(configuration) != null;
    }

    public override void Draw(T configuration, int? labelSize = null, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration) ?? "";
        ImGui.SetNextItemWidth(labelSize ?? this.LabelSize);
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

        ImGui.SetNextItemWidth(inputSize ?? this.InputSize);
        if (ImGui.InputText("##" + this.Key + "Input", ref value, 500))
        {
            this.UpdateFilterConfiguration(configuration, value);
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