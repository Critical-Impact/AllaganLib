using System;
using System.Collections.Generic;
using System.Numerics;

using AllaganLib.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace AllaganLib.Monitors.Debuggers;

public unsafe class ShopEventHandlerDebugPane : DebugLogPane
{
    public override void DrawInfo()
    {
        ImGui.Text("Current ShopEventHandler IDs:");
        ImGui.Separator();
        var ids = this.GetCurrentShopEventHandlerIds();

        if (ids.Count == 0)
        {
            ImGui.Text("No active ShopEventHandler found.");
        }
        else
        {
            foreach (var (source, id) in ids)
                ImGui.Text($"- {id} (from {source})");
        }
    }

    /// <summary>
    /// Collects all shop IDs from the AgentShop and the ShopEventHandler.AgentProxy if available.
    /// Returns a list of tuples containing the source and the ID.
    /// </summary>
    private List<(string Source, uint Id)> GetCurrentShopEventHandlerIds()
    {
        var ids = new List<(string Source, uint Id)>();

        try
        {
            // Check the active AgentShop
            var agent = (AgentShop*)UIModule.Instance()->GetAgentModule()->GetAgentByInternalId(AgentId.Shop);
            if (agent != null && agent->IsAgentActive() && agent->EventReceiver != null && agent->IsAddonReady())
            {
                var proxy = (ShopEventHandler.AgentProxy*)agent->EventReceiver;
                if (proxy != null && proxy->Handler != null)
                {
                    ids.Add(("AgentShop", proxy->Handler->Info.EventId.Id));
                }
            }

            // Check the singleton AgentProxy
            var agentProxy = ShopEventHandler.AgentProxy.Instance();
            if (agentProxy != null && agentProxy->Handler != null)
            {
                ids.Add(("AgentProxy", agentProxy->Handler->Info.EventId.Id));
            }
        }
        catch (Exception ex)
        {
            this.AddLog($"Error retrieving ShopEventHandler IDs: {ex.Message}");
        }

        return ids;
    }

    public override string Name => "Shop Event Handlers";
}