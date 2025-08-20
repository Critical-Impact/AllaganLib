using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.FormFields;

public abstract class BooleanFormField<T> : FormField<bool, T>
    where T : IConfigurable<bool?>
{
    protected BooleanFormField(ImGuiService imGuiService)
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

    public override bool DrawInput(T configuration, int? inputSize = null)
    {
        var currentValue = this.CurrentValue(configuration);
        var wasUpdated = false;

        if (ImGui.Checkbox("##" + this.Key + "Boolean", ref currentValue))
        {
            if (currentValue != this.CurrentValue(configuration))
            {
                if (this.AutoSave)
                {
                    this.UpdateFilterConfiguration(configuration, currentValue);
                }

                wasUpdated = true;
            }
        }

        return wasUpdated;
    }

    public override FormFieldType FieldType => FormFieldType.Boolean;
}