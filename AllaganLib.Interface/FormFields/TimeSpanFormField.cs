using System;
using AllaganLib.Interface.Services;
using AllaganLib.Interface.Widgets;
using ImGuiNET;

namespace AllaganLib.Interface.FormFields;

public abstract class TimeSpanFormField<T> : FormField<(TimeUnit, int)?, T>
    where T : IConfigurable<(TimeUnit, int)?>
{
    private readonly TimeSpanPickerWidget timeSpanPickerWidget;

    public TimeSpanFormField(TimeSpanPickerWidget timeSpanPickerWidget, ImGuiService imGuiService) : base(imGuiService)
    {
        this.timeSpanPickerWidget = timeSpanPickerWidget;
    }

    public override (TimeUnit, int)? CurrentValue(T configuration)
    {
        return configuration.Get(this.Key) ?? this.DefaultValue;
    }

    public override void DrawInput(T configuration, int? inputSize = null)
    {
        var currentValue = this.CurrentValue(configuration);
        var timeUnit = currentValue?.Item1 ?? null;
        var timeValue = currentValue?.Item2 ?? null;

        if (this.timeSpanPickerWidget.Draw(this.Key + "Input", ref timeUnit, ref timeValue, inputSize))
        {
            if (timeUnit == null || timeValue == null)
            {
                this.UpdateFilterConfiguration(configuration, null);
            }
            else
            {
                this.UpdateFilterConfiguration(configuration, (timeUnit.Value, timeValue.Value));
            }
        }
    }

    public override void UpdateFilterConfiguration(T configuration, (TimeUnit, int)? newValue)
    {
        configuration.Set(this.Key, newValue);
    }
}