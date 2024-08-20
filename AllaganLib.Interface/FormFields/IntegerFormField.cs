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

    public override void DrawInput(T configuration, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration).ToString();
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
    }
}