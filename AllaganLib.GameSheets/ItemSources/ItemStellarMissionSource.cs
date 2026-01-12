using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemStellarMissionSource : ItemSource
{
    public ItemStellarMissionSource(ItemRow itemRow, uint qtyRequired, RowRef<WKSMissionUnit> mission, RowRef<WKSMissionToDo> todo, RowRef<WKSItemInfo> itemInfo)
        : base(ItemInfoType.StellarMission)
    {
        this.Item = itemRow;
        this.Quantity = qtyRequired;
        this.Mission = mission;
        this.Todo = todo;
        this.ItemInfo = itemInfo;
    }

    public override uint Quantity { get; }

    public RowRef<WKSMissionUnit> Mission { get; }

    public RowRef<WKSMissionToDo> Todo { get; }

    public RowRef<WKSItemInfo> ItemInfo { get; }

    public override RelationshipType RelationshipType => RelationshipType.Rewards;
}