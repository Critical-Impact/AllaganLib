using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCraftLeveSource : ItemSource
{
    private readonly int rowIndex;

    public CraftLeveRow CraftLeveRow { get; }

    public ItemCraftLeveSource(CraftLeveRow craftLeveRow, int rowIndex, ItemRow item) : base(ItemInfoType.CraftLeve)
    {
        this.rowIndex = rowIndex;
        this.CraftLeveRow = craftLeveRow;
        this.Item = item;
    }

    public override uint Quantity => this.CraftLeveRow.Base.ItemCount[this.rowIndex];

}