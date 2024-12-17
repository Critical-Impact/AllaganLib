using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GilShopRow : ExtendedRow<GilShop, GilShopRow, GilShopSheet>, IShop
{
    private List<ENpcBaseRow>? eNpcs;
    private string? _name;
    private List<GilShopItemRow>? gilShopItems;
    private IEnumerable<ItemRow>? items;
    private IEnumerable<ItemRow>? costItems;
    private HashSet<uint>? mapIds;

    public string Name => this.ToString();

    public IEnumerable<IShopListing> ShopListings => this.GilShopItems;

    public HashSet<uint> MapIds => this.mapIds ??= this.ENpcs.SelectMany(c => c.Locations.Select(d => d.Map.RowId)).Distinct().ToHashSet();

    public override string ToString()
    {
        if (this._name == null)
        {
            this._name = this.Sheet.GetShopName(this.RowId) ?? this.Base.Name.ExtractText();
            if (this._name == string.Empty)
            {
                if (this.ENpcs.Any())
                {
                    this._name = this.ENpcs.First().ENpcResidentRow.Base.Singular.ExtractText();
                }
            }
        }

        return this._name;
    }

    public List<GilShopItemRow> GilShopItems
    {
        get { return this.gilShopItems ??= this.Sheet.GetGilShopItems(this.RowId) ?? []; }
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
        get { return this.items ??= this.GilShopItems.Select(c => c.Item); }
    }

    public IEnumerable<ItemRow> CostItems
    {
        get { return this.costItems ??= new List<ItemRow>(); }
    }
}