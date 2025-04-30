using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemDungeonBossDropSource : ItemDungeonSource
{
    private HashSet<uint>? mapIds;

    public DungeonBoss DungeonBoss { get; }

    public DungeonBossDrop DungeonBossDrop { get; }

    public BNpcNameRow BNpcName { get; }

    public ItemDungeonBossDropSource(ItemRow itemRow, ContentFinderConditionRow contentFinderConditionRow, BNpcNameRow bNpcName, DungeonBoss dungeonBoss, DungeonBossDrop dungeonBossDrop)
        : base(contentFinderConditionRow, ItemInfoType.DungeonBossDrop)
    {
        this.Item = itemRow;
        this.DungeonBoss = dungeonBoss;
        this.DungeonBossDrop = dungeonBossDrop;
        this.BNpcName = bNpcName;
        if (this.ContentFinderCondition.Base.TerritoryType.ValueNullable != null)
        {
            this.mapIds = [this.ContentFinderCondition.Base.TerritoryType.Value.Map.RowId];
        }
    }

    public override HashSet<uint>? MapIds => this.mapIds;

    public override uint Quantity => this.DungeonBossDrop.Quantity;

}