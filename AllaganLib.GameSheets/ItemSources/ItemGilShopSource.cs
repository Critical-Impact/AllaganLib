using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGilShopSource : ItemShopSource
{
    private readonly List<ItemRow> costItems;
    private readonly List<ItemRow> items;

    public GilShopItemRow GilShopItem { get; }

    public GilShopRow GilShop { get; }

    public override List<ItemRow> Items => items;

    public override List<ItemRow> CostItems => costItems;

    public ItemGilShopSource(GilShopItemRow gilShopItem, GilShopRow gilShop, ItemInfoType shopType = ItemInfoType.GilShop)
        : base(gilShop, shopType)
    {
        this.GilShopItem = gilShopItem;
        this.GilShop = gilShop;
        this.Item = gilShopItem.Item;
        this.CostItem = gilShopItem.Costs.First().Item;
        this.costItems = gilShopItem.Costs.Select(c => c.Item).ToList();
        this.items = gilShopItem.Rewards.Select(c => c.Item).ToList();
    }

    public uint Cost => this.Item.Base.PriceMid;

    public override uint Quantity => 1;


    public override HashSet<uint>? MapIds => this.GilShop.MapIds;
}