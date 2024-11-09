using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemDungeonDropSource : ItemDungeonSource
{
    public DungeonDrop DungeonDrop { get; }


    public ItemDungeonDropSource(ItemRow itemRow, ContentFinderConditionRow contentFinderConditionRow, DungeonDrop dungeonDrop)
        : base(contentFinderConditionRow, ItemInfoType.DungeonDrop)
    {
        this.Item = itemRow;
        this.DungeonDrop = dungeonDrop;
    }

    public override uint Quantity => 1;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}