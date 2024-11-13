using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemDungeonBossChestSource : ItemDungeonSource
{
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
    }

    public override uint Quantity => this.DungeonBossChest.Quantity;

}