using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;

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
        return this.CurrentValue(configuration) != this.DefaultValue;
    }

    public override bool DrawInput(T configuration, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration) ?? "";
        var wasUpdated = false;

        ImGui.SetNextItemWidth(inputSize ?? this.InputSize);
        if (ImGui.InputText("##" + this.Key + "Input", ref value, 500))
        {
            if (this.AutoSave)
            {
                this.UpdateFilterConfiguration(configuration, value);
            }

            wasUpdated = true;
        }

        return wasUpdated;
    }

    public override FormFieldType FieldType => FormFieldType.String;
}