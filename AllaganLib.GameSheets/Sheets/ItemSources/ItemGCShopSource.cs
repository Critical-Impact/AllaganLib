using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemGCShopSource : ItemShopSource
{
    private readonly GCScripShopItemRow fccShopListing;
    private readonly GCShopRow gcShop;

    public ItemGCShopSource(GCScripShopItemRow fccShopListing, GCShopRow gcShop) : base(gcShop, ItemInfoType.GCShop)
    {
        this.fccShopListing = fccShopListing;
        this.gcShop = gcShop;
        this.Item = this.fccShopListing.Item;
        this.CostItem = this.fccShopListing.Costs.FirstOrDefault()?.Item;
    }

    public override uint Quantity => this.fccShopListing.Count;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;

    public GCScripShopItemRow FccShopListing => this.fccShopListing;

    public GCShopRow GcShop => this.gcShop;

    public override HashSet<uint>? MapIds => this.gcShop.MapIds;
}