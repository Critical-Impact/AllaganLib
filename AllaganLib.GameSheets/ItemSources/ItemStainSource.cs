using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemStainSource : ItemSource
{
    public RowRef<Stain> Stain { get; }

    public ItemStainSource(ItemRow itemRow, RowRef<Stain> stain)
        : base(ItemInfoType.Stain)
    {
        this.Stain = stain;
        this.Item = itemRow;
    }

    public override uint Quantity => 0;
}