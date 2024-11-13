using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemDungeonChestSource : ItemDungeonSource
{
    public DungeonChestItem DungeonChestItem { get; }

    public DungeonChest DungeonChest { get; }

    public ItemDungeonChestSource(ItemRow itemRow, ContentFinderConditionRow contentFinderConditionRow, DungeonChestItem dungeonChestItem, DungeonChest dungeonChest)
        : base(contentFinderConditionRow, ItemInfoType.DungeonChest)
    {
        this.DungeonChestItem = dungeonChestItem;
        this.DungeonChest = dungeonChest;
        this.Item = itemRow;
    }

    public override uint Quantity => 1;

}