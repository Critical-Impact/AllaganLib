using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using ImGuiNET;

namespace AllaganLib.Interface.FormFields;

public abstract class IntegerFormField<T> : FormField<int, T>
    where T : IConfigurable<int?>
{
    public IntegerFormField(ImGuiService imGuiService)
        : base(imGuiService)
    {
    }

    public virtual string? Affix { get; }

    public override int CurrentValue(T configurable)
    {
        return configurable.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(T configurable, int newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public override void Draw(T configuration, int? labelSize = null, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration).ToString();
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
        if (ImGui.InputText("##" + this.Key + "Input", ref value, 100, ImGuiInputTextFlags.CharsDecimal))
        {
            int parsedNumber;
            if (int.TryParse(value, out parsedNumber))
            {
                this.UpdateFilterConfiguration(configuration, parsedNumber);
            }
        }

        if (this.Affix != null)
        {
            ImGui.SameLine();
            ImGui.Text(this.Affix);
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