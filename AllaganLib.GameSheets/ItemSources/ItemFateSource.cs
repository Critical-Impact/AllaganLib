using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemFateSource : ItemSource
{
    private FateRow fateRow;

    public ItemFateSource(ItemRow item, FateRow fate) : base(ItemInfoType.Fate)
    {
        this.Item = item;
        this.fateRow = fate;
    }

    public FateRow Fate => this.fateRow;

    public override uint Quantity => 1;

}