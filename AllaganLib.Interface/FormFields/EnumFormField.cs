using System;
using System.Linq;
using AllaganLib.Interface.Services;

namespace AllaganLib.Interface.FormFields;

public abstract class EnumFormField<TEnum, TConfigurable> : ChoiceFormField<Enum, TConfigurable>
    where TEnum : Enum, IComparable
    where TConfigurable : IConfigurable<Enum?>
{
    protected EnumFormField(ImGuiService imGuiService) : base(imGuiService)
    {
    }

    public new TEnum CurrentValue(TConfigurable configurable)
    {
        return (TEnum)base.CurrentValue(configurable);
    }

    public override string GetFormattedChoice(Enum choice)
    {
        return this.Choices.SingleOrDefault(c => ((TEnum)c.Key).Equals((TEnum)choice)).Value;
    }

    public void UpdateFilterConfiguration(TConfigurable configurable, TEnum? newValue)
    {
        base.UpdateFilterConfiguration(configurable, newValue);
    }

    public override bool HasValueSet(TConfigurable configuration)
    {
        var currentValue = this.CurrentValue(configuration);
        return !currentValue.Equals((TEnum)this.DefaultValue);
    }
}