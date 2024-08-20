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

    public override void DrawInput(T configuration, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration) ?? "";
        ImGui.SetNextItemWidth(inputSize ?? this.InputSize);
        if (ImGui.InputText("##" + this.Key + "Input", ref value, 500))
        {
            this.UpdateFilterConfiguration(configuration, value);
        }
    }
}