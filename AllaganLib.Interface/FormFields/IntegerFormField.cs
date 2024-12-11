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

    public override bool DrawInput(T configuration, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration).ToString();
        var wasUpdated = false;

        ImGui.SetNextItemWidth(inputSize ?? this.InputSize);
        if (ImGui.InputText("##" + this.Key + "Input", ref value, 100, ImGuiInputTextFlags.CharsDecimal))
        {
            int parsedNumber;
            if (int.TryParse(value, out parsedNumber))
            {
                if (this.AutoSave)
                {
                    this.UpdateFilterConfiguration(configuration, parsedNumber);
                }

                wasUpdated = true;
            }
        }

        if (this.Affix != null)
        {
            ImGui.SameLine();
            ImGui.Text(this.Affix);
        }

        return wasUpdated;
    }
}