using System.Numerics;
using AllaganLib.Interface.Services;

namespace AllaganLib.Interface.FormFields;

public abstract class FormField<T, TS> : IFormField<TS>
{
    public ImGuiService ImGuiService { get; }

    public FormField(ImGuiService imGuiService)
    {
        this.ImGuiService = imGuiService;
    }

    public abstract T DefaultValue { get; set; }

    public virtual int LabelSize { get; set; } = 400;

    public virtual int InputSize { get; set; } = 300;

    public abstract T CurrentValue(TS configuration);

    public abstract void Draw(TS configuration, int? labelSize = null, int? inputSize = null);

    public abstract void UpdateFilterConfiguration(TS configuration, T? newValue);

    public abstract string Key { get; set; }

    public abstract string Name { get; set; }

    public abstract string HelpText { get; set; }

    public bool HideReset { get; set; } = false;

    public bool ColourModified { get; set; } = true;

    public abstract string Version { get; }

    public virtual string? Image { get; } = null;

    public virtual Vector2? ImageSize { get; } = null;

    public virtual string WizardName => this.Name;

    public virtual bool HasValueSet(TS configuration)
    {
        var currentValue = this.CurrentValue(configuration);
        if (currentValue == null && this.DefaultValue == null)
        {
            return false;
        }

        return !currentValue?.Equals(this.DefaultValue) ?? true;
    }

    public virtual void Reset(TS configuration)
    {
        this.UpdateFilterConfiguration(configuration, this.DefaultValue);
    }
}