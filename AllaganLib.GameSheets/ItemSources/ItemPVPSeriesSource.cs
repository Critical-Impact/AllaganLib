using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemPVPSeriesSource : ItemSource
{
    private readonly int index;

    public RowRef<PvPSeries> PvpSeries { get; }

    public int Level { get; }

    public ItemPVPSeriesSource(ItemRow itemRow, RowRef<PvPSeries> pvpSeriesRowRef, int level, int index)
        : base(ItemInfoType.PVPSeries)
    {
        this.index = index;
        this.PvpSeries = pvpSeriesRowRef;
        this.Level = level;
        this.Item = itemRow;
    }

    public PvPSeries.LevelRewardsStruct LevelRewards => this.PvpSeries.Value.LevelRewards[this.Level];

    public override uint Quantity => this.PvpSeries.Value.LevelRewards[this.Level].LevelRewardCount[this.index];
}