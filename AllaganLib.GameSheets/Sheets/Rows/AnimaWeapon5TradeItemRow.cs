using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class AnimaWeapon5TradeItemRow : ExtendedRow<AnimaWeapon5TradeItem, AnimaWeapon5TradeItemRow, AnimaWeapon5TradeItemSheet>, IShop
{
    private string? name;
    private List<ENpcBaseRow>? eNpcs;
    private List<AnimaWeapon5TradeItemListing>? shopListings;
    private List<ItemRow>? items;
    private List<ItemRow>? costItems;
    private HashSet<uint>? mapIds;

    public string Name => this.ToString();

    public IEnumerable<IShopListing> ShopListings => this.shopListings ??= this.BuildShopListings();

    public IEnumerable<AnimaWeapon5TradeItemListing> AnimaWeapon5TradeItemListings => this.shopListings ??= this.BuildShopListings();

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
            return this.items ??= this.AnimaWeapon5TradeItemListings.SelectMany(c => c.Rewards.Select(d => d.Item)).ToList();
        }
    }

    public IEnumerable<ItemRow> CostItems
    {
        get
        {
            return this.costItems ??= this.AnimaWeapon5TradeItemListings.SelectMany(c => c.Costs.Select(d => d.Item)).ToList();
        }
    }

    public override string ToString()
    {
        if (this.name != null)
        {
            return this.name;
        }

        this.name = this.Base.Category.Value.Name.ToImGuiString();
        if (this.name == string.Empty)
        {
            this.name = "Unknown Vendor";
        }

        return this.name;
    }

    private List<AnimaWeapon5TradeItemListing> BuildShopListings()
    {
        return new List<AnimaWeapon5TradeItemListing>()
        {
            new(this.Sheet.GetItemSheet(), this.Base),
        };
    }

    public class AnimaWeapon5TradeItemListing : IShopListing
    {
        public List<IShopListingItem> Cost { get; }

        public IShopListingItem Reward { get; }

        public AnimaWeapon5TradeItemListing(
            ItemSheet itemSheet,
            AnimaWeapon5TradeItem tradeItem)
        {
            this.Cost = new();
            for (var index = 0; index < tradeItem.Item.Count; index++)
            {
                var item = tradeItem.Item[index];
                if (item.RowId == 0)
                {
                    continue;
                }
                var qty = tradeItem.Quantity[index];
                var isHq = tradeItem.IsHQ[index];
                this.Cost.Add(new ShopListingItem(itemSheet, this, item.RowId, qty, isHq));
            }

            this.Reward = new ShopListingItem(itemSheet, this, tradeItem.CrystalSand.RowId, tradeItem.ReceiveQuantity);
        }

        public IEnumerable<IShopListingItem> Costs => this.Cost;

        public IEnumerable<IShopListingItem> Rewards
        {
            get { yield return this.Reward; }
        }
    }
}