using System;
using System.Collections.Generic;
using System.Numerics;
using AllaganLib.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Event;

namespace AllaganLib.Monitors.Debuggers;

public unsafe class EventHandlerModuleDebugPane : DebugLogPane
{
    private readonly ITargetManager targetManager;

    public EventHandlerModuleDebugPane(ITargetManager targetManager)
    {
        this.targetManager = targetManager;
    }

    public override void DrawInfo()
    {
        ImGui.Text("EventHandlerModule Contents:");
        ImGui.Separator();

        var eventHandlers = this.GetEventHandlerModuleContents();

        if (eventHandlers.Count == 0)
        {
            ImGui.Text("No EventHandlerModule active or no handlers found.");
        }
        else
        {
            foreach (var handlerInfo in eventHandlers)
            {
                ImGui.TextUnformatted(handlerInfo);
            }
        }
    }

    /// <summary>
    /// Reads the EventHandlerModule from EventFramework and returns a list of string representations.
    /// </summary>
    private List<string> GetEventHandlerModuleContents()
    {
        var results = new List<string>();

        try
        {
            var eventFramework = EventFramework.Instance();
            if (eventFramework == null)
            {
                return results;
            }

            var module = eventFramework->EventHandlerModule;
            foreach (var eventHandler in module.EventHandlerMap)
            {
                string line = $"EventId: {eventHandler.Item1}";
                results.Add(line);
                foreach (var eventObject in eventHandler.Item2.Value->EventObjects)
                {
                    if (this.targetManager.Target?.DataId == eventObject.Value->BaseId)
                    {
                        results.Add($"Currently related to target {this.targetManager.Target.EntityId}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this.AddLog($"Error reading EventHandlerModule: {ex.Message}");
        }

        return results;
    }

    public override string Name => "EventHandlerModule";
}