using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

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

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}