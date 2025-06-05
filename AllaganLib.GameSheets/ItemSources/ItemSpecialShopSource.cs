using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
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

    public ItemSpecialShopSource(ItemRow reward, ItemRow? cost, IShopListingItem shopListingItem, IShopListing shopListing, SpecialShopRow specialShop)
        : base(specialShop, ItemInfoType.SpecialShop)
    {
        this.shopListing = shopListing;
        this.specialShop = specialShop;
        this.shopListingItem = shopListingItem;
        this.Item = reward;
        this.CostItem = cost;
    }

    public override uint Quantity => this.shopListingItem.Count;

    /// <inheritdoc/>
    protected override IReadOnlyList<ItemInfo> CreateCostItems()
    {
        return ItemInfo.FromShopListing(this.shopListing.Costs);
    }

    /// <inheritdoc/>
    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        return ItemInfo.FromShopListing(this.shopListing.Rewards);
    }

    public override HashSet<uint>? MapIds => this.SpecialShop.MapIds;
}