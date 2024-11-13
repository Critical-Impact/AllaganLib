using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemFccShopSource : ItemShopSource
{
    private readonly FccShopRow.FccShopListing fccShopListing;
    private readonly FccShopRow fccShop;

    public FccShopRow.FccShopListing FccShopListing => this.fccShopListing;

    public FccShopRow FccShop => this.fccShop;

    public ItemFccShopSource(FccShopRow.FccShopListing fccShopListing, FccShopRow fccShop)
        : base(fccShop, ItemInfoType.FCShop)
    {
        this.fccShopListing = fccShopListing;
        this.fccShop = fccShop;
        this.Item = fccShopListing.Reward.Item;
        this.CostItem = fccShopListing.Cost.Item;
    }

    public uint Cost => this.fccShopListing.Cost.Count;

    public override uint Quantity => this.fccShopListing.Reward.Count;


    public override HashSet<uint>? MapIds => this.FccShop.MapIds;
}