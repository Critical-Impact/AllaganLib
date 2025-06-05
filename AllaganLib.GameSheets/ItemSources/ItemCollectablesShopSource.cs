using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCollectablesShopSource : ItemShopSource
{
    private readonly CollectablesShopRow.CollectablesShopListing collectablesShopListing;
    private readonly CollectablesShopRow collectablesShop;

    public CollectablesShopRow.CollectablesShopListing CollectablesShopListing => this.collectablesShopListing;

    public CollectablesShopRow CollectablesShop => this.collectablesShop;

    public ItemCollectablesShopSource(CollectablesShopRow.CollectablesShopListing collectablesShopListing, CollectablesShopRow collectablesShop)
        : base(collectablesShop, ItemInfoType.CollectablesShop)
    {
        this.collectablesShopListing = collectablesShopListing;
        this.collectablesShop = collectablesShop;
        this.Item = collectablesShopListing.Reward.Item;
        this.CostItem = collectablesShopListing.Cost.Item;
    }

    public uint Cost => this.collectablesShopListing.Cost.Count;

    public override uint Quantity => this.collectablesShopListing.Reward.Count;


    public override HashSet<uint>? MapIds => this.CollectablesShop.MapIds;
}