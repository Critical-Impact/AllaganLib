using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGCSupplyDutySource : ItemSource
{
    private readonly uint quantity;

    public GCSupplyDutyRow GCSupplyDutyRow { get; }

    public GCSupplyDutyRewardRow? DailySupplyRewardRow => this.GCSupplyDutyRow.GetGCSupplyDutyRewardRow();

    public ItemGCSupplyDutySource(GCSupplyDutyRow gcSupplyDutyRow, uint quantity, ItemRow itemRow)
        : base(ItemInfoType.GCDailySupply)
    {
        this.quantity = quantity;
        this.GCSupplyDutyRow = gcSupplyDutyRow;
        this.Item = itemRow;
    }

    public override uint Quantity => this.quantity;

    public byte RecipeLevel => (byte)this.GCSupplyDutyRow.RowId;

}