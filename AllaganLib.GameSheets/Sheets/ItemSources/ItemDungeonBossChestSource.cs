using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

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

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}