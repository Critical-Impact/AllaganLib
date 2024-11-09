using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

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
    }

    public override uint Quantity => this.shopListingItem.Count;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;

    public override List<ItemRow> Items => this.shopListing.Rewards.Select(c => c.Item).ToList();

    public override List<ItemRow> CostItems => this.shopListing.Costs.Select(c => c.Item).ToList();

    public override HashSet<uint>? MapIds => this.SpecialShop.MapIds;
}