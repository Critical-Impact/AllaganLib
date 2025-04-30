using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemDungeonBossChestSource : ItemDungeonSource
{
    private HashSet<uint>? mapIds;

    public DungeonBoss DungeonBoss { get; }

    public DungeonBossChest DungeonBossChest { get; }

    public BNpcNameRow BNpcName { get; }

    public ItemDungeonBossChestSource(ItemRow itemRow, ContentFinderConditionRow contentFinderConditionRow, BNpcNameRow bNpcName, DungeonBoss dungeonBoss, DungeonBossChest dungeonBossChest)
        : base(contentFinderConditionRow, ItemInfoType.DungeonBossChest)
    {
        this.Item = itemRow;
        this.DungeonBoss = dungeonBoss;
        this.DungeonBossChest = dungeonBossChest;
        this.BNpcName = bNpcName;
        if (this.ContentFinderCondition.Base.TerritoryType.ValueNullable != null)
        {
            this.mapIds = [this.ContentFinderCondition.Base.TerritoryType.Value.Map.RowId];
        }
    }

    public override HashSet<uint>? MapIds => this.mapIds;

    public override uint Quantity => this.DungeonBossChest.Quantity;

}