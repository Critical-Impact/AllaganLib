using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemDailySupplyItemSource : ItemSource
{
    private readonly int supplyIndex;

    public DailySupplyItemRow DailySupplyItem { get; }

    public GCSupplyDutyRewardRow? DailySupplyRewardRow =>
        this.DailySupplyItem.GetGCSupplyDutyRewardRow(this.supplyIndex);

    public ItemDailySupplyItemSource(DailySupplyItemRow dailySupplyItem, int supplyIndex, ItemRow itemRow)
        : base(ItemInfoType.GCDailySupply)
    {
        this.supplyIndex = supplyIndex;
        this.DailySupplyItem = dailySupplyItem;
        this.Item = itemRow;
    }

    public override uint Quantity => this.DailySupplyItem.Base.Quantity[this.supplyIndex];

    public byte RecipeLevel => this.DailySupplyItem.Base.RecipeLevel[this.supplyIndex];

}