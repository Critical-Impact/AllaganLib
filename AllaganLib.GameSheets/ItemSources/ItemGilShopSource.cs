using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGilShopSource : ItemShopSource
{
    public GilShopItemRow GilShopItem { get; }

    public GilShopRow GilShop { get; }

    public ItemGilShopSource(GilShopItemRow gilShopItem, GilShopRow gilShop, ItemInfoType shopType = ItemInfoType.GilShop)
        : base(gilShop, shopType)
    {
        this.GilShopItem = gilShopItem;
        this.GilShop = gilShop;
        this.Item = gilShopItem.Item;
        this.CostItem = gilShopItem.Costs.First().Item;
    }

    public uint Cost => this.Item.Base.PriceMid;

    public override uint Quantity => 1;


    public override HashSet<uint>? MapIds => this.GilShop.MapIds;
}