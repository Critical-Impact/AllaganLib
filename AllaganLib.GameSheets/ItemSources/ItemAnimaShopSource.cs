using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemAnimaShopSource : ItemShopSource
{
    private readonly IShopListing shopListing;
    private readonly IShopListingItem shopListingItem;
    private readonly AnimaWeapon5TradeItemRow animaWeaponTradeItem;

    public IShopListing ShopListing => this.shopListing;

    public IShopListingItem ShopListingItem => this.shopListingItem;

    public AnimaWeapon5TradeItemRow AnimaWeaponTradeItem => this.animaWeaponTradeItem;

    public ItemAnimaShopSource(ItemRow reward, ItemRow? cost, IShopListingItem shopListingItem, IShopListing shopListing, AnimaWeapon5TradeItemRow animaWeaponTradeItem)
        : base(animaWeaponTradeItem, ItemInfoType.AnimaShop)
    {
        this.shopListing = shopListing;
        this.animaWeaponTradeItem = animaWeaponTradeItem;
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

    public override HashSet<uint>? MapIds => this.AnimaWeaponTradeItem.MapIds;
}