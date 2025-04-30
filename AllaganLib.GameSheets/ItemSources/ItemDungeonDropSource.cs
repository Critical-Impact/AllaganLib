using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemDungeonDropSource : ItemDungeonSource
{
    private HashSet<uint>? mapIds;

    public DungeonDrop DungeonDrop { get; }


    public ItemDungeonDropSource(ItemRow itemRow, ContentFinderConditionRow contentFinderConditionRow, DungeonDrop dungeonDrop)
        : base(contentFinderConditionRow, ItemInfoType.DungeonDrop)
    {
        this.Item = itemRow;
        this.DungeonDrop = dungeonDrop;
        if (this.ContentFinderCondition.Base.TerritoryType.ValueNullable != null)
        {
            this.mapIds = [this.ContentFinderCondition.Base.TerritoryType.Value.Map.RowId];
        }
    }

    public override HashSet<uint>? MapIds => this.mapIds;

    public override uint Quantity => 1;

}