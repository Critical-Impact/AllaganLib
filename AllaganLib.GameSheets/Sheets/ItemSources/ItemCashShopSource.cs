using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemCashShopSource : ItemSource
{
    private readonly FittingShopItemSetRow? fittingShopItemSetRow;

    public ItemCashShopSource(ItemRow item, FittingShopItemSetRow? fittingShopItemSetRow) : base(ItemInfoType.CashShop)
    {
        this.fittingShopItemSetRow = fittingShopItemSetRow;
        this.Item = item;
    }

    public override uint Quantity => 1;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}