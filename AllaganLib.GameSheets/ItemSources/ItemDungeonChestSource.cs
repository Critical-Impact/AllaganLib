using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemDungeonChestSource : ItemDungeonSource
{
    private HashSet<uint>? mapIds;

    public DungeonChestItem DungeonChestItem { get; }

    public DungeonChest DungeonChest { get; }

    public ItemDungeonChestSource(ItemRow itemRow, ContentFinderConditionRow contentFinderConditionRow, DungeonChestItem dungeonChestItem, DungeonChest dungeonChest)
        : base(contentFinderConditionRow, ItemInfoType.DungeonChest)
    {
        this.DungeonChestItem = dungeonChestItem;
        this.DungeonChest = dungeonChest;
        this.Item = itemRow;
        if (this.ContentFinderCondition.Base.TerritoryType.ValueNullable != null)
        {
            this.mapIds = [this.ContentFinderCondition.Base.TerritoryType.Value.Map.RowId];
        }
    }

    public override HashSet<uint>? MapIds => this.mapIds;

    public override uint Quantity => 1;

}