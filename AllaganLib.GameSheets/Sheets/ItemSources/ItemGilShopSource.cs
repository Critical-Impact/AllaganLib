using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemGilShopSource : ItemShopSource
{
    public GilShopItemRow GilShopItem { get; }

    public GilShopRow GilShop { get; }

    public ItemGilShopSource(GilShopItemRow gilShopItem, GilShopRow gilShop, ItemInfoType shopType)
        : base(gilShop, shopType)
    {
        this.GilShopItem = gilShopItem;
        this.GilShop = gilShop;
        this.Item = gilShopItem.Item;
        this.CostItem = gilShopItem.Costs.First().Item;
    }

    public uint Cost => this.Item.Base.PriceMid;

    public override uint Quantity => 1;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;

    public override HashSet<uint>? MapIds => this.GilShop.MapIds;
}