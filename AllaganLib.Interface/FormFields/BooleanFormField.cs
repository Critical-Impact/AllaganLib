using AllaganLib.Interface.Services;
using Dalamud.Interface.Colors;
using ImGuiNET;

namespace AllaganLib.Interface.FormFields;

public abstract class BooleanFormField<T> : FormField<bool, T>
    where T : IConfigurable<bool?>
{
    private readonly string[] Choices = new[] { "N/A", "Yes", "No" };

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

    public override void DrawInput(T configuration, int? inputSize = null)
    {
        var currentValue = this.CurrentValue(configuration);

        if (ImGui.Checkbox("##" + this.Key + "Boolean", ref currentValue))
        {
            if (currentValue != this.CurrentValue(configuration))
            {
                this.UpdateFilterConfiguration(configuration, currentValue);
            }
        }
    }
}