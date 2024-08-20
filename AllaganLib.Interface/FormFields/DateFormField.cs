using System;
using System.Globalization;

using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using ImGuiNET;

namespace AllaganLib.Interface.FormFields;

public abstract class DateFormField<T> : FormField<DateTime?, T>
    where T : IConfigurable<DateTime?>
{
    public DateFormField(ImGuiService imGuiService)
        : base(imGuiService)
    {
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

    public override void DrawInput(T configuration, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration)?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
        if (ImGui.InputText("##" + this.Key + "Input", ref value, 500))
        {
            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var date))
            {
                this.UpdateFilterConfiguration(configuration, date);
            }
        }
    }
}