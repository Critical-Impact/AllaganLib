using System.Numerics;
using AllaganLib.Interface.Services;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.FormFields;

public abstract class FloatRangeFormField<TConfiguration> : FormField<(float Min, float Max)?, TConfiguration>
    where TConfiguration : IConfigurable<(float Min, float Max)?>
{
    public float MinimumValue { get; }

    public float MaximumValue { get; }

    public FloatRangeFormField(float minimumValue, float maximumValue, ImGuiService imGuiService)
        : base(imGuiService)
    {
        this.MinimumValue = minimumValue;
        this.MaximumValue = maximumValue;
    }

    public override (float Min, float Max)? CurrentValue(TConfiguration configuration)
    {
        return configuration.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(TConfiguration configuration, (float Min, float Max)? newValue)
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

        var range = new Vector2(value.Min, value.Max);

        if (ImGui.SliderFloat2("##" + this.Key, ref range, this.MinimumValue, this.MaximumValue))
        {
            if (this.AutoSave)
            {
                this.UpdateFilterConfiguration(configuration, (range.X, range.Y));
            }

            wasUpdated = true;
        }


        return wasUpdated;
    }

    public override FormFieldType FieldType => FormFieldType.FloatRange;
}