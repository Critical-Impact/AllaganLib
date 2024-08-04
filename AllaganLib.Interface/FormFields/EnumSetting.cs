// <copyright file="EnumSetting.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using AllaganLib.Interface.Services;

namespace AllaganLib.Interface.FormFields;

public abstract class EnumSetting<T, TS> : ChoiceSetting<Enum, TS>
    where T : Enum, IComparable
    where TS : IConfigurable<Enum?>
{
    protected EnumSetting(ImGuiService imGuiService) : base(imGuiService)
    {
    }

    public T CurrentValue(TS configurable)
    {
        return (T)base.CurrentValue(configurable);
    }

    public void UpdateFilterConfiguration(TS configurable, T? newValue)
    {
        base.UpdateFilterConfiguration(configurable, newValue);
    }
}