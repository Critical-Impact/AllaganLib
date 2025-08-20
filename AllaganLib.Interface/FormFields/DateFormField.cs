using System;
using AllaganLib.Interface.Services;
using AllaganLib.Interface.Widgets;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.FormFields;

public abstract class DateFormField<T> : FormField<DateTime?, T>
    where T : IConfigurable<DateTime?>
{
    private readonly DatePickerWidget datePickerWidget;

    public DateFormField(DatePickerWidget datePickerWidget, ImGuiService imGuiService)
        : base(imGuiService)
    {
        this.datePickerWidget = datePickerWidget;
    }
    
    public override DateTime? CurrentValue(T configurable)
    {
        return configurable.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(T configurable, DateTime? newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public override bool HasValueSet(T configuration)
    {
        return this.CurrentValue(configuration) != null;
    }

    public override bool DrawInput(T configuration, int? inputSize = null)
    {
        var currentValue = this.CurrentValue(configuration);
        var wasUpdated = false;

        if (this.datePickerWidget.Draw("##" + this.Key + "Input", ref currentValue, inputSize))
        {
            if (this.AutoSave)
            {
                this.UpdateFilterConfiguration(configuration, currentValue);
            }

            wasUpdated = true;
        }

        return wasUpdated;
    }

    public override FormFieldType FieldType => FormFieldType.Date;
}