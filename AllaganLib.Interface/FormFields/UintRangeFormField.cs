using System.Numerics;
using AllaganLib.Interface.Services;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.FormFields;

public abstract class UintRangeFormField<TConfiguration> : FormField<(uint Min, uint Max)?, TConfiguration>
    where TConfiguration : IConfigurable<(uint Min, uint Max)?>
{
    public uint MinimumValue { get; }

    public uint MaximumValue { get; }

    public UintRangeFormField(uint minimumValue, uint maximumValue, ImGuiService imGuiService)
        : base(imGuiService)
    {
        this.MinimumValue = minimumValue;
        this.MaximumValue = maximumValue;
    }

    public override (uint Min, uint Max)? CurrentValue(TConfiguration configuration)
    {
        return configuration.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(TConfiguration configuration, (uint Min, uint Max)? newValue)
    {
        configuration.Set(this.Key, newValue);
    }

    public override bool HasValueSet(TConfiguration configuration)
    {
        return this.CurrentValue(configuration) != this.DefaultValue;
    }

    public override bool DrawInput(TConfiguration configuration, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration) ?? (this.MinimumValue, this.MaximumValue);
        var wasUpdated = false;

        ImGui.SetNextItemWidth(inputSize ?? this.InputSize);

        var min = (int)value.Min;
        var max = (int)value.Max;

        if (ImGui.DragIntRange2("##" + this.Key, ref min, ref max, 1, (int)this.MinimumValue, (int)this.MaximumValue))
        {
            if (this.AutoSave)
            {
                this.UpdateFilterConfiguration(configuration, ((uint)min, (uint)max));
            }

            wasUpdated = true;
        }


        return wasUpdated;
    }

    public override FormFieldType FieldType => FormFieldType.UintRange;
}