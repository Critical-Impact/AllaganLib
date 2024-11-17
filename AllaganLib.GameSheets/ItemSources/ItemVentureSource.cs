using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemVentureSource : ItemSource
{
    private readonly RetainerTaskRow retainerTaskRow;

    public ItemVentureSource(ItemRow itemRow, ItemRow venture, RetainerTaskRow retainerTaskRow, ItemInfoType ventureType)
        : base(ventureType)
    {
        this.Item = itemRow;
        this.CostItem = venture;
        this.retainerTaskRow = retainerTaskRow;
    }

    public RetainerTaskRow RetainerTaskRow => this.retainerTaskRow;

    public override uint Quantity => this.retainerTaskRow.Quantity;

}