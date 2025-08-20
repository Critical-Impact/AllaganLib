using System;
using System.Collections.Generic;
using System.Numerics;
using AllaganLib.Monitors.Interfaces;
using AllaganLib.Monitors.Services;
using AllaganLib.Shared.Extensions;
using AllaganLib.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;

namespace AllaganLib.Monitors.Debuggers;

public class ShopMonitorDebugPane : DebugLogPane
{
    private readonly IShopMonitorService shopMonitor;

    public ShopMonitorDebugPane(IShopMonitorService shopMonitor)
    {
        this.shopMonitor = shopMonitor;
        this.shopMonitor.OnShopChanged += this.HandleShopChanged;
        this.shopMonitor.OnShopOpened += this.HandleShopOpened;
        this.shopMonitor.OnShopClosed += this.HandleShopClosed;
    }

    private void HandleShopChanged()
    {
        var shopInfo = this.shopMonitor.GetCurrentShopType();
        if (shopInfo is { } info)
        {
            string npcInfo = $"NPC: {info.Npc.ENpcResidentRow.Base.Singular.ToImGuiString()} (ID: {info.Npc.RowId})";
            if (info.ActiveShop is { } active)
            {
                this.AddLog($"[Changed] {npcInfo} | Active shop: '{active.Name}' (ID: {active.RowId})");
            }
            else
            {
                this.AddLog($"[Changed] {npcInfo} | No active shop");
            }
        }
        else
        {
            this.AddLog("[Changed] No shop state available");
        }
    }

    private void HandleShopOpened()
    {
        var shopInfo = this.shopMonitor.GetCurrentShopType();
        if (shopInfo is { } info)
        {
            string npcInfo = $"NPC: {info.Npc.ENpcResidentRow.Base.Singular.ToImGuiString()} (ID: {info.Npc.RowId})";
            if (info.ActiveShop is { } active)
            {
                this.AddLog($"[Opened] {npcInfo} | Shop opened: '{active.Name}' (ID: {active.RowId})");
            }
            else
            {
                this.AddLog($"[Opened] {npcInfo} | Shop opened (no active shop)");
            }
        }
        else
        {
            this.AddLog("[Opened] Shop window opened (no shop info)");
        }
    }

    private void HandleShopClosed()
    {
        var shopInfo = this.shopMonitor.GetCurrentShopType();
        if (shopInfo is { } info)
        {
            string npcInfo = $"NPC: {info.Npc.ENpcResidentRow.Base.Singular.ToImGuiString()} (ID: {info.Npc.RowId})";
            if (info.ActiveShop is { } active)
            {
                this.AddLog($"[Closed] {npcInfo} | Shop closed: '{active.Name}' (ID: {active.RowId})");
            }
            else
            {
                this.AddLog($"[Closed] {npcInfo} | Shop closed (no active shop)");
            }
        }
        else
        {
            this.AddLog("[Closed] Shop window closed (no shop info)");
        }
    }

    public override void DrawInfo()
    {
        var shopInfo = this.shopMonitor.GetCurrentShopType();

        if (shopInfo is null)
        {
            ImGui.Text("No shop active.");
        }
        else
        {
            var (npc, shops, activeShop) = shopInfo.Value;
            ImGui.Text($"NPC: {npc.ENpcResidentRow.Base.Singular.ToImGuiString()} (ID: {npc.RowId})");
            ImGui.Separator();

            ImGui.Text("Shops:");
            foreach (var shop in shops)
            {
                bool isActive = activeShop != null && shop.RowId == activeShop.RowId;
                string shopDisplay = $"- {shop.Name} (ID: {shop.RowId})";

                if (isActive)
                {
                    ImGui.TextColored(new Vector4(0.2f, 1f, 0.2f, 1f), shopDisplay);
                }
                else
                {
                    ImGui.Text(shopDisplay);
                }
            }
        }
    }

    public void Dispose()
    {
        this.shopMonitor.OnShopChanged -= this.HandleShopChanged;
        this.shopMonitor.OnShopOpened -= this.HandleShopOpened;
        this.shopMonitor.OnShopClosed -= this.HandleShopClosed;
    }

    public override string Name => "Shop Monitor";
}
