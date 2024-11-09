using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GCShopRow : ExtendedRow<GCShop, GCShopRow, GCShopSheet>, IShop
{
    private List<ENpcBaseRow>? eNpcs;
    private List<GCScripShopItemRow>? shopListings;
    private IEnumerable<ItemRow>? items;
    public string? name;
    private HashSet<uint>? mapIds;

    public string Name => this.ToString();

    public IEnumerable<IShopListing> ShopListings => this.shopListings ??= this.BuildShopListings();

    public IEnumerable<GCScripShopItemRow> GCShopListings => this.shopListings ??= this.BuildShopListings();

    public HashSet<uint> MapIds => this.mapIds ??= this.ENpcs.SelectMany(c => c.Locations.Select(d => d.Map.RowId)).Distinct().ToHashSet();

    private List<GCScripShopItemRow> BuildShopListings()
    {
        var categories = this.Sheet.GetGCScripShopCategorySheet().GetCategoriesByGrandCompanyId(this.Base.GrandCompany.RowId);
        return this.shopListings = categories.SelectMany(c => this.Sheet.GetGCScripShopItemSheet().GetItemsByGCShopId(c.RowId) ?? []).ToList();
    }

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
        get { return this.items ??= this.ShopListings.SelectMany(c => c.Rewards.Select(d => d.Item)); }
    }

    public IEnumerable<ItemRow> CostItems
    {
        get { return this.items ??= this.ShopListings.SelectMany(c => c.Costs.Select(d => d.Item)); }
    }

    public override string ToString()
    {
        if (this.name != null)
        {
            return this.name;
        }

        var shopName = this.Sheet.GetShopName(this.RowId);
        this.name = shopName ?? this.Base.GrandCompany.Value.Name.ExtractText();
        if (this.name == string.Empty)
        {
            this.name = "Unknown Vendor";
        }

        return this.name;
    }
}