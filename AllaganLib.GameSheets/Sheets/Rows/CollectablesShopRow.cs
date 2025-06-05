using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class CollectablesShopRow : ExtendedRow<CollectablesShop, CollectablesShopRow, CollectablesShopSheet>, IShop
{
    private string? name;
    private List<ENpcBaseRow>? eNpcs;
    private List<CollectablesShopListing>? shopListings;
    private List<ItemRow>? items;
    private List<ItemRow>? costItems;
    private HashSet<uint>? mapIds;

    public string Name => this.ToString();

    public IEnumerable<IShopListing> ShopListings => this.shopListings ??= this.BuildShopListings();

    public IEnumerable<CollectablesShopListing> CollectablesShopListings => this.shopListings ??= this.BuildShopListings();

    public HashSet<uint> MapIds => this.mapIds ??= this.ENpcs.SelectMany(c => c.Locations.Select(d => d.Map.RowId)).Distinct().ToHashSet();

    public IEnumerable<ENpcBaseRow> ENpcs
    {
        get
        {
            return this.eNpcs ??= this.Sheet.GetShopIds(this.RowId)
                .Select(c => this.Sheet.GetENpcBaseSheet().GetRow(c)).ToList();
        }
    }

    public IEnumerable<ItemRow> Items
    {
        get
        {
            return this.items ??= this.Base.ShopItems.SelectMany(c => c.Value).Where(c => c.Item.RowId != 0)
                .Select(c => this.Sheet.GetItemSheet().GetRow(c.Item.RowId)).ToList();
        }
    }

    public IEnumerable<ItemRow> CostItems
    {
        get { return this.items ??= [this.Sheet.GetItemSheet().GetRow(HardcodedItems.FreeCompanyCreditItemId)]; }
    }

    public override string ToString()
    {
        if (this.name != null)
        {
            return this.name;
        }

        var shopName = this.Sheet.GetShopName(this.RowId);
        this.name = shopName ?? this.Base.Name.ExtractText();
        if (this.name == string.Empty)
        {
            this.name = "Unknown Vendor";
        }

        return this.name;
    }

    private List<CollectablesShopListing> BuildShopListings()
    {
        var listings = new List<CollectablesShopListing>();
        foreach (var subrowRef in this.Base.ShopItems)
        {
            foreach (var item in subrowRef.Value)
            {
                if (item.Item.RowId == 0)
                {
                    continue;
                }

                var rewardItem = new RowRef<CollectablesShopRewardItem>(
                    this.Sheet.GameData.Excel,
                    item.CollectablesShopRewardScrip.RowId);

                if (!rewardItem.IsValid || rewardItem.Value.Item.RowId == 0)
                {
                    continue;
                }


                listings.Add(new CollectablesShopListing(this.Sheet.GetItemSheet(), item, rewardItem.Value));
            }
        }

        return listings.ToList();
    }

    public class CollectablesShopListing : IShopListing
    {
        public IShopListingItem Cost { get; }

        public IShopListingItem Reward { get; }

        public CollectablesShopListing(
            ItemSheet itemSheet,
            CollectablesShopItem shopItem,
            CollectablesShopRewardItem rewardItem)
        {
            this.Cost = new ShopListingItem(itemSheet, this, shopItem.Item.RowId, 1);
            this.Reward = new ShopListingItem(itemSheet, this, rewardItem.Item.RowId, rewardItem.RewardLow);
        }

        public IEnumerable<IShopListingItem> Costs
        {
            get { yield return this.Cost; }
        }

        public IEnumerable<IShopListingItem> Rewards
        {
            get { yield return this.Reward; }
        }
    }
}