using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GilShopItemRow : ExtendedSubrow<GilShopItem, GilShopItemRow, GilShopItemSheet>, IShopListing, IShopListingItem
{
    private IShopListingItem? costListingItem;
    private ItemRow? itemRow;

    public IEnumerable<IShopListingItem> Rewards
    {
        get { yield return this; }
    }

    public IEnumerable<IShopListingItem> Costs
    {
        get
        {
            yield return this.costListingItem ??= new ShopListingItem(
                this.Sheet.GetItemSheet(),
                this,
                1,
                this.Item.Base.PriceMid,
                false,
                0);
        }
    }

    public ItemRow Item => this.Sheet.GetItemSheet().GetRow(this.Base.Item.RowId);

    public uint Count => 1;

    public bool? IsHq => null;

    public uint? CollectabilityRating => null;

    public uint? FCRankRequired => null;
}