using System;
using AllaganLib.Interface.Services;

namespace AllaganLib.Interface.FormFields;

public abstract class EnumFormField<TEnum, TConfigurable> : ChoiceFormField<Enum, TConfigurable>
    where TEnum : Enum, IComparable
    where TConfigurable : IConfigurable<Enum?>
{
    protected EnumFormField(ImGuiService imGuiService) : base(imGuiService)
    {
    }

    public TEnum CurrentValue(TConfigurable configurable)
    {
        return (TEnum)base.CurrentValue(configurable);
    }

    public void UpdateFilterConfiguration(TConfigurable configurable, TEnum? newValue)
    {
        base.UpdateFilterConfiguration(configurable, newValue);
    }
}