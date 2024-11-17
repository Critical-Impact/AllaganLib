using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCashShopSource : ItemSource
{
    public FittingShopItemSetRow? FittingShopItemSetRow { get; }

    public ItemCashShopSource(ItemRow item, FittingShopItemSetRow? fittingShopItemSetRow) : base(ItemInfoType.CashShop)
    {
        this.FittingShopItemSetRow = fittingShopItemSetRow;
        this.Item = item;
    }

    public override uint Quantity => 1;

}