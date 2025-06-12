using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Interfaces;
using AllaganLib.Shared.Services;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AllaganLib.Interface.Services;

public class HotkeyService<TConfiguration> : HostedFrameworkService
    where TConfiguration : IConfigurable<VirtualKey[]?>
{
    private readonly IEnumerable<IHotkey<TConfiguration>> hotkeys;
    private readonly TConfiguration configuration;
    private readonly ExcelSheet<EventItem> eventItemSheet;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly IGameGui gameGui;
    private readonly IKeyState keyState;

    public HotkeyService(IEnumerable<IHotkey<TConfiguration>> hotkeys, TConfiguration configuration, ILogger<HostedFrameworkService> logger, ExcelSheet<EventItem> eventItemSheet, ExcelSheet<Item> itemSheet, IGameGui gameGui, IFramework framework, IKeyState keyState)
        : base(logger, framework)
    {
        this.hotkeys = hotkeys;
        this.configuration = configuration;
        this.eventItemSheet = eventItemSheet;
        this.itemSheet = itemSheet;
        this.gameGui = gameGui;
        this.keyState = keyState;
    }

    public bool CheckHotkeyState(VirtualKey[] keys, bool clearOnPressed = true)
    {
        foreach (var vk in this.keyState.GetValidVirtualKeys())
        {
            if (keys.Contains(vk))
            {
                if (!this.keyState[vk])
                {
                    return false;
                }
            }
            else
            {
                if (this.keyState[vk])
                {
                    return false;
                }
            }
        }

        if (clearOnPressed)
        {
            foreach (var k in keys)
            {
                this.keyState[(int)k] = false;
            }
        }

        return true;
    }

    public override void FrameworkOnUpdate(IFramework framework)
    {
        try
        {
            object? item = null;
            var id = this.gameGui.HoveredItem;
            if (id != 0)
            {
                if (id >= 2000000)
                {
                    item = this.eventItemSheet.GetRowOrDefault((uint)id);
                }
                else
                {
                    item = this.itemSheet.GetRowOrDefault((uint)(id % 500000));
                }
            }

            foreach (var h in this.hotkeys)
            {
                if (!h.IsEnabled || h.IsEditing)
                {
                    continue;
                }

                if (h.CurrentValue(this.configuration).Length == 0)
                {
                    continue;
                }

                if (!this.CheckHotkeyState(h.CurrentValue(this.configuration)))
                {
                    continue;
                }

                if (h is IRegularHotkey<TConfiguration> regularHotkey)
                {
                    regularHotkey.OnTriggered();
                }

                if (item == null)
                {
                    continue;
                }

                if (id >= 2000000)
                {
                    if (h is IEventItemHotkey<TConfiguration> eventItemHotkey)
                    {
                        eventItemHotkey.OnTriggered(item as EventItem?);
                    }
                }
                else
                {
                    if (h is IItemHotkey<TConfiguration> itemHotkey)
                    {
                        itemHotkey.OnTriggered(item as Item?);
                    }
                }

                break;
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Hotkey failed to trigger.");
        }
    }
}