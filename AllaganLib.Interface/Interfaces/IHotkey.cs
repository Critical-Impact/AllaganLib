// <copyright file="IHotkey.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.Interface.FormFields;
using Dalamud.Game.ClientState.Keys;
using Lumina.Excel.Sheets;

namespace AllaganLib.Interface.Interfaces;

public interface IHotkey<TConfiguration> : IFormField<TConfiguration>
    where TConfiguration : IConfigurable<VirtualKey[]?>
{
    public VirtualKey[] CurrentValue(TConfiguration configurable);

    public bool IsEditing { get; }

    public bool IsEnabled { get; }
}

public interface IItemHotkey<TConfiguration> : IHotkey<TConfiguration>
    where TConfiguration : IConfigurable<VirtualKey[]?>
{
    public void OnTriggered(Item? item);
}

public interface IEventItemHotkey<TConfiguration> : IHotkey<TConfiguration>
    where TConfiguration : IConfigurable<VirtualKey[]?>
{
    public void OnTriggered(EventItem? item);
}

public interface IRegularHotkey<TConfiguration> : IHotkey<TConfiguration>
    where TConfiguration : IConfigurable<VirtualKey[]?>
{
    public void OnTriggered();
}