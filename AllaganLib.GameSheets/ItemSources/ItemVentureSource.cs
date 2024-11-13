using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemVentureSource : ItemSource
{
    private readonly RetainerTaskRow retainerTaskRow;

    public ItemVentureSource(ItemRow itemRow, RetainerTaskRow retainerTaskRow, ItemInfoType ventureType)
        : base(ventureType)
    {
        this.Item = itemRow;
        this.retainerTaskRow = retainerTaskRow;
    }

    public RetainerTaskRow RetainerTaskRow => this.retainerTaskRow;

    public override uint Quantity => this.retainerTaskRow.Quantity;

}