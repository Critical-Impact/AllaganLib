using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GCScripShopItemRow : ExtendedSubrow<GCScripShopItem, GCScripShopItemRow, GCScripShopItemSheet>, IShopListing, IShopListingItem
{
    private ShopListingItem? cost;
    private ItemRow? item;

    public GCScripShopCategoryRow Category => this.Sheet.GetGCScripShopCategorySheet().GetRow(this.RowId);

    public IEnumerable<IShopListingItem> Rewards
    {
        get { yield return this; }
    }

    public IEnumerable<IShopListingItem> Costs
    {
        get
        {
            yield return this.cost ??= new ShopListingItem(
                this.Sheet.GetItemSheet(),
                this,
                this.Category.GrandCompany.ItemId,
                this.Base.CostGCSeals);
        }
    }

    public ItemRow Item => this.item ??= this.Sheet.GetItemSheet().GetRow(this.Base.Item.RowId);

    public uint Count => this.Base.CostGCSeals;

    public bool? IsHq => null;

    public uint? CollectabilityRating => null;

    public uint? FCRankRequired => null;
}