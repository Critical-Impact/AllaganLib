using System;

using AllaganLib.Interface.Services;
using AllaganLib.Interface.Widgets;

namespace AllaganLib.Interface.FormFields;

public abstract class DateRangeFormField<T> : FormField<(DateTime, DateTime)?, T>
    where T : IConfigurable<(DateTime, DateTime)?>
{
    private readonly DateRangePickerWidget dateRangePickerWidget;

    public DateRangeFormField(DateRangePickerWidget dateRangePickerWidget, ImGuiService imGuiService) : base(imGuiService)
    {
        this.dateRangePickerWidget = dateRangePickerWidget;
    }

    public override (DateTime, DateTime)? CurrentValue(T configuration)
    {
        return configuration.Get(this.Key) ?? this.DefaultValue;
    }

    public override void DrawInput(T configuration, int? inputSize = null)
    {
        var currentValue = this.CurrentValue(configuration);
        var startDate = currentValue?.Item1;
        var endDate = currentValue?.Item2;
        if (this.dateRangePickerWidget.Draw("##" + this.Key + "Input", ref startDate, ref endDate, inputSize))
        {
            if (startDate == null || endDate == null)
            {
                this.UpdateFilterConfiguration(configuration, null);
            }
            else
            {
                this.UpdateFilterConfiguration(configuration, (startDate.Value, endDate.Value));
            }
        }
    }

    public override void UpdateFilterConfiguration(T configuration, (DateTime, DateTime)? newValue)
    {
        configuration.Set(this.Key, newValue);
    }
}