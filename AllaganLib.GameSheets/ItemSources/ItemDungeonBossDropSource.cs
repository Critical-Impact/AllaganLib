using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemDungeonBossDropSource : ItemDungeonSource
{
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
    }

    public override uint Quantity => this.DungeonBossDrop.Quantity;

}