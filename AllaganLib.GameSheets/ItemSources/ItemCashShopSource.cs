using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCashShopSource : ItemSource
{
    public StoreItem StoreItem { get; }

    public FittingShopItemSetRow? FittingShopItemSetRow { get; }

    public decimal PriceUsd => (decimal)this.StoreItem.PriceCentsUSD / 100;

    public ItemCashShopSource(ItemRow item, StoreItem storeItem, FittingShopItemSetRow? fittingShopItemSetRow)
        : base(ItemInfoType.CashShop)
    {
        this.StoreItem = storeItem;
        this.FittingShopItemSetRow = fittingShopItemSetRow;
        this.Item = item;
    }

    public override uint Quantity => 1;

}