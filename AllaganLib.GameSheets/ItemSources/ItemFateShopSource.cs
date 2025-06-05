using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemFateShopSource : ItemShopSource
{
    private readonly IShopListing shopListing;
    private readonly IShopListingItem shopListingItem;
    private readonly SpecialShopRow specialShop;
    private readonly FateShopRow fateShopRow;

    public IShopListing ShopListing => this.shopListing;

    public IShopListingItem ShopListingItem => this.shopListingItem;

    public SpecialShopRow SpecialShop => this.specialShop;

    public FateShopRow FateShop => this.fateShopRow;

    public ItemFateShopSource(IShopListingItem shopListingItem, IShopListing shopListing, FateShopRow fateShopRow, SpecialShopRow specialShop)
        : base(specialShop, ItemInfoType.FateShop)
    {
        this.shopListing = shopListing;
        this.specialShop = specialShop;
        this.fateShopRow = fateShopRow;
        this.shopListingItem = shopListingItem;
        this.Item = shopListingItem.Item;
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