using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.Shared.Extensions;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class InclusionShopRow : ExtendedRow<InclusionShop, InclusionShopRow, InclusionShopSheet>, IShop
{
    private List<ENpcBaseRow>? eNpcs;
    private IEnumerable<ItemRow>? rewards;
    private IEnumerable<ItemRow>? costs;

    public string Name => this.Base.Unknown0.ToImGuiString();

    public IEnumerable<IShopListing> ShopListings { get; set; }

    public IEnumerable<ENpcBaseRow> ENpcs
    {
        get
        {
            if (this.eNpcs == null)
            {
                    this.eNpcs = this.Sheet.GetShopIds(this.RowId)
                        .Select(c => this.Sheet.GetENpcBaseSheet().GetRow(c)).ToList();
            }

            return this.eNpcs;
        }
    }

    public IEnumerable<ItemRow> Items => this.rewards ??= this.BuildShopItems().Rewards;

    public IEnumerable<ItemRow> CostItems => this.costs ??= this.BuildShopItems().Costs;

    private (SpecialShopListing[] ShopListings, List<ItemRow> Rewards, List<ItemRow> Costs) BuildShopItems()
    {
        var shopListingsLookup = new List<SpecialShopListing>();
        var rewardItemsLookup = new List<ItemRow>();
        var costItemsLookup = new List<ItemRow>();
        foreach (var item in this.Base.Category)
        {
            if (item.ValueNullable == null)
            {
                continue;
            }

            if (item.Value.InclusionShopSeries.ValueNullable == null)
            {
                continue;
            }
            foreach (var shopSeries in item.Value.InclusionShopSeries.Value)
            {
                var specialShop = this.Sheet.GetSpecialShopSheet().GetRowOrDefault(shopSeries.SpecialShop.RowId);
                if (specialShop == null)
                {
                    continue;
                }
                shopListingsLookup.AddRange(specialShop.SpecialShopListings);
                rewardItemsLookup.AddRange(specialShop.Items);
                costItemsLookup.AddRange(specialShop.CostItems);
            }
        }

        return (shopListingsLookup.ToArray(), rewardItemsLookup, costItemsLookup);
    }
}