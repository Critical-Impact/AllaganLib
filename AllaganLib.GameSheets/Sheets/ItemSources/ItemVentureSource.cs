using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

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

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}