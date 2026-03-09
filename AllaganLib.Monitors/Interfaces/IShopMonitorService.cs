using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Monitors.Enums;
using AllaganLib.Monitors.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.Monitors.Interfaces;

/// <summary>
/// Tracks the user's current shop providing events for when their shop changes and the IDs of the related npc, shops(in the case of an npc providing multiple "shops") and active shop.
/// </summary>
public interface IShopMonitorService
{
    /// <summary>
    /// Represents a method that handles when a shop window either opens or closes.
    /// </summary>
    public delegate void ShopChangedDelegate();

    /// <summary>
    /// Represents a method that handles when a shop window opens.
    /// </summary>
    public delegate void ShopOpenedDelegate();

    /// <summary>
    /// Represents a method that handles when a shop window closes.
    /// </summary>
    public delegate void ShopClosedDelegate();

    /// <summary>
    /// Occurs when a shop window is either opened or closed.
    /// </summary>
    event ShopChangedDelegate? OnShopChanged;

    /// <summary>
    /// Occurs when a shop window is opened.
    /// </summary>
    event ShopOpenedDelegate? OnShopOpened;

    /// <summary>
    /// Occurs when a shop window is closed.
    /// </summary>
    event ShopClosedDelegate? OnShopClosed;

    /// <summary>
    /// If the player is currently viewing a shop, provides the NPC, shop entries and active shop.
    /// </summary>
    /// <returns>A tuple containing the npc, shops and active shop or null if no shop.</returns>
    public (ENpcBaseRow Npc, List<List<IShop>> Shops, List<List<IShop>>? SubShops, IShop? ActiveShop)? GetCurrentShopType();

    /// <summary>
    /// Given a shop ID and type, returns the shop if available.
    /// </summary>
    /// <param name="shopId">The ID of the shop.</param>
    /// <param name="type">The type of the shop.</param>
    /// <returns>The shop if found.</returns>
    public IShop? GetShopByIdAndType(uint shopId, ShopType type);

    /// <summary>
    /// If the player is at a NPC that has shops available, returns a list of shop types and the IDs of the shops along with the active shop's type and ID.
    /// </summary>
    /// <returns>Returns a list of shop types and the IDs of the shops along with the active shop's type and ID.</returns>
    public (ENpcBaseRow ENpcBase, List<IShopMenu> MenuItems, List<IShopMenu>? SubmenuItems, (ShopType, uint)? ActiveShopId)? GetCurrentShopTypeIds();
}

public interface IShopMenu
{
    /// <summary>
    /// All the shops related to this menu item, may only be 1 item.
    /// </summary>
    List<(ShopType ShopType, uint ShopId)> Shops { get; }

    /// <summary>
    /// If this menu item opens a new submenu, returns the related topic select.
    /// </summary>
    RowRef<TopicSelect>? TopicSelect { get; }

    bool IsActive { get; }
}

public class ShopMenu : IShopMenu
{
    public ShopMenu(List<(ShopType ShopType, uint ShopId)> shops, bool isActive, RowRef<TopicSelect>? topicSelect = null)
    {
        this.Shops = shops;
        this.TopicSelect = topicSelect;
        this.IsActive = isActive;
    }

    public List<(ShopType ShopType, uint ShopId)> Shops { get; }

    public RowRef<TopicSelect>? TopicSelect { get; }

    public bool IsActive { get; }
}