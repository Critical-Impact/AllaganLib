using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class SpecialShopRow : ExtendedRow<SpecialShop, SpecialShopRow, SpecialShopSheet>, IShop
{
    private IEnumerable<IShopListing>? shopListings;
    private IEnumerable<SpecialShopListing>? specialShopListings;
    private IEnumerable<ItemRow>? rewards;
    private IEnumerable<ItemRow>? costs;
    private List<ENpcBaseRow>? eNpcs;
    private FateShopRow? fateShopRow;
    private string? name;
    private HashSet<uint>? mapIds;
    
    public string Name => this.ToString();
    
    public IEnumerable<IShopListing> ShopListings => this.shopListings ??= this.BuildShopItems().ShopListings;
    
    public IEnumerable<IShopListing> SpecialShopListings => this.specialShopListings ??= this.BuildShopItems().ShopListings;
    
    public HashSet<uint> MapIds => this.mapIds ??= this.ENpcs.SelectMany(c => c.Locations.Select(d => d.Map.RowId)).Distinct().ToHashSet();
    
    public IEnumerable<ENpcBaseRow> ENpcs =>
        this.eNpcs ??= this.Sheet.GetShopIds(this.RowId)
            .Select(c => this.Sheet.GetENpcBaseSheet().GetRow(c)).ToList();
    
    public IEnumerable<ItemRow> Items => this.rewards ??= this.BuildShopItems().Rewards;
    
    public IEnumerable<ItemRow> CostItems => this.costs ??= this.BuildShopItems().Costs;
    
    public FateShopRow? FateShop => this.GetFateShopAdjustedRowId() == null ? null : this.fateShopRow ??= this.Sheet.GetFateShopSheet().GetRow(this.GetFateShopAdjustedRowId()!.Value);
    
    public bool IsFateShop => this.GetFateShopAdjustedRowId() != null;
    
    private uint? GetFateShopAdjustedRowId()
    {
        var adjustedId = this.Sheet.GetSpecialShopToFateShop(this.RowId);
        return adjustedId;
    }
    
    public override string ToString()
    {
        if (this.name == null)
        {
            var adjustedRowId = this.GetFateShopAdjustedRowId();
            if (adjustedRowId != null)
            {
                var resident = this.Sheet.GameData.GetExcelSheet<ENpcResident>()!.GetRowOrDefault(adjustedRowId.Value);
                if (resident != null)
                {
                    this.name = resident.Value.Singular.ExtractText();
                }
            }
            
            if (this.name == null)
            {
                var shopName = this.Sheet.GetShopName(this.GetFateShopAdjustedRowId() ?? this.RowId);
                this.name = shopName ?? this.Base.Name.ExtractText();
            }
        }
        
        if (this.name == string.Empty)
        {
            this.name = "Unknown Vendor";
        }
        
        return this.name;
    }
    
    private (SpecialShopListing[] ShopListings, List<ItemRow> Rewards, List<ItemRow> Costs) BuildShopItems()
    {
        var shopListingsLookup = new List<SpecialShopListing>();
        var rewardItemsLookup = new List<ItemRow>();
        var costItemsLookup = new List<ItemRow>();
        foreach (var item in this.Base.Item)
        {
            var specialShopListing = new SpecialShopListing(this, this.Sheet.GetItemSheet(), this.Sheet.TomestoneLookup, item);
            if (specialShopListing.Rewards.Any())
            {
                shopListingsLookup.Add(specialShopListing);
                rewardItemsLookup.AddRange(specialShopListing.Rewards.Select(listing => listing.Item));
                costItemsLookup.AddRange(specialShopListing.Costs.Select(listing => listing.Item));
            }
        }
        
        return (shopListingsLookup.ToArray(), rewardItemsLookup, costItemsLookup);
    }
}