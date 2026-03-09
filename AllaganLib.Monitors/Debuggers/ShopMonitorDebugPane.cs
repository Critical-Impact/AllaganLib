using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Monitors.Enums;
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
        var shopInfo = this.shopMonitor.GetCurrentShopTypeIds();

        if (shopInfo is null)
        {
            ImGui.Text("No shop active.");
        }
        else
        {
            var npc = shopInfo.Value.ENpcBase;
            var menuItems = shopInfo.Value.MenuItems;
            var subMenuItems = shopInfo.Value.SubmenuItems;
            var activeShop = shopInfo.Value.ActiveShopId;

            ImGui.Text($"NPC: {npc.ENpcResidentRow.Base.Singular.ToImGuiString()} (ID: {npc.RowId})");
            ImGui.Separator();

            ImGui.Text("Menu Items:");
            foreach (var menuItem in menuItems)
            {
                bool isActive = menuItem.IsActive;

                string shopDisplay;
                if (menuItem.TopicSelect != null)
                {
                    shopDisplay = $"- {menuItem.TopicSelect.Value.Value.Name.ToImGuiString()} (Prehandler ID: {menuItem.TopicSelect.Value.RowId})";
                }
                else if(menuItem.Shops.Count > 0)
                {
                    var shop = this.shopMonitor.GetShopByIdAndType(menuItem.Shops.First().ShopId, menuItem.Shops.First().ShopType);
                    if (shop != null)
                    {
                        shopDisplay = $"- {shop.Name} (ID: {shop.RowId})";
                    }
                    else
                    {
                        shopDisplay = "- Shop not found";
                    }
                }
                else
                {
                    shopDisplay = "- No shops available";
                }

                if (isActive)
                {
                    ImGui.TextColored(new Vector4(0.2f, 1f, 0.2f, 1f), shopDisplay);
                }
                else
                {
                    ImGui.Text(shopDisplay);
                }
            }

            ImGui.Text("Submenu Items:");
            if (subMenuItems != null)
            {
                foreach (var menuItem in subMenuItems)
                {
                    bool isActive = menuItem.IsActive;

                    string shopDisplay;
                    if (menuItem.TopicSelect != null)
                    {
                        shopDisplay =
                            $"- {menuItem.TopicSelect.Value.Value.Name.ToImGuiString()} (Prehandler ID: {menuItem.TopicSelect.Value.RowId})";
                    }
                    else if (menuItem.Shops.Count > 0)
                    {
                        var shop = this.shopMonitor.GetShopByIdAndType(
                            menuItem.Shops.First().ShopId,
                            menuItem.Shops.First().ShopType);
                        if (shop != null)
                        {
                            shopDisplay = $"- {shop.Name} (ID: {shop.RowId})";
                        }
                        else
                        {
                            shopDisplay = "- Shop not found";
                        }
                    }
                    else
                    {
                        shopDisplay = "- No shops available";
                    }

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
    }

    public void Dispose()
    {
        this.shopMonitor.OnShopChanged -= this.HandleShopChanged;
        this.shopMonitor.OnShopOpened -= this.HandleShopOpened;
        this.shopMonitor.OnShopClosed -= this.HandleShopClosed;
    }

    public override string Name => "Shop Monitor";
}
