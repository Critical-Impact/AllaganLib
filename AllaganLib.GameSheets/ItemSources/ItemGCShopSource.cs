using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGCShopSource : ItemShopSource
{
    private readonly GCScripShopItemRow gcScripShopItem;
    private readonly GCShopRow gcShop;

    public ItemGCShopSource(GCScripShopItemRow gcScripShopItem, GCShopRow gcShop) : base(gcShop, ItemInfoType.GCShop)
    {
        this.gcScripShopItem = gcScripShopItem;
        this.gcShop = gcShop;
        this.Item = this.gcScripShopItem.Item;
        this.CostItem = this.gcScripShopItem.Costs.FirstOrDefault()?.Item;
    }

    public override uint Quantity => this.gcScripShopItem.Count;


    public GCScripShopItemRow GCScripShopItem => this.gcScripShopItem;

    public GCShopRow GcShop => this.gcShop;

    public override HashSet<uint>? MapIds => this.gcShop.MapIds;
}