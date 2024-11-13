using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class FccShopRow : ExtendedRow<FccShop, FccShopRow, FccShopSheet>, IShop
{
    private string? name;
    private List<ENpcBaseRow>? eNpcs;
    private List<FccShopListing>? shopListings;
    private List<ItemRow>? items;
    private List<ItemRow>? costItems;
    private HashSet<uint>? mapIds;

    public string Name => this.ToString();

    public IEnumerable<IShopListing> ShopListings => this.shopListings ??= this.BuildShopListings();

    public IEnumerable<FccShopListing> FccShopListings => this.shopListings ??= this.BuildShopListings();

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
            return this.items ??= this.Base.ItemData.Where(c => c.Item.RowId != 0)
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

    private List<FccShopListing> BuildShopListings()
    {
        var listings = new List<FccShopListing>();
        foreach (var item in this.Base.ItemData)
        {
            if (item.Item.RowId == 0)
            {
                continue;
            }

            listings.Add(new FccShopListing(this.Sheet.GetItemSheet(), item));
        }

        return listings.ToList();
    }

    public class FccShopListing : IShopListing
    {
        public IShopListingItem Cost { get; }

        public IShopListingItem Reward { get; }

        public FccShopListing(
            ItemSheet itemSheet,
            FccShop.ItemDataStruct itemDataStruct)
        {
            const uint costItem = HardcodedItems.FreeCompanyCreditItemId;
            this.Cost = new ShopListingItem(itemSheet, this, costItem, itemDataStruct.Cost);
            this.Reward = new ShopListingItem(itemSheet, this, itemDataStruct.Item.RowId, 1);
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