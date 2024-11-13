using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemSpecialShopSource : ItemShopSource
{
    private readonly IShopListing shopListing;
    private readonly IShopListingItem shopListingItem;
    private readonly SpecialShopRow specialShop;

    public IShopListing ShopListing => this.shopListing;

    public IShopListingItem ShopListingItem => this.shopListingItem;

    public SpecialShopRow SpecialShop => this.specialShop;

    public ItemSpecialShopSource(IShopListingItem shopListingItem, IShopListing shopListing, SpecialShopRow specialShop)
        : base(specialShop, ItemInfoType.SpecialShop)
    {
        this.shopListing = shopListing;
        this.specialShop = specialShop;
        this.shopListingItem = shopListingItem;
        this.Item = shopListingItem.Item;
        this.CostItem = shopListing.Costs.FirstOrDefault()?.Item;
    }

    public override uint Quantity => this.shopListingItem.Count;

    public override List<ItemRow> Items => this.shopListing.Rewards.Select(c => c.Item).ToList();

    public override List<ItemRow> CostItems => this.shopListing.Costs.Select(c => c.Item).ToList();

    public override HashSet<uint>? MapIds => this.SpecialShop.MapIds;
}