using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemFishingSource : ItemSource
{
    private readonly FishingSpotRow fishingSpotRow;

    public ItemFishingSource(FishingSpotRow fishingSpotRow, ItemRow itemRow)
        : base(ItemInfoType.Fishing)
    {
        this.fishingSpotRow = fishingSpotRow;
        this.Item = itemRow!;
    }

    public override uint Quantity => 1;


    public FishingSpotRow FishingSpotRow => this.fishingSpotRow;

    public override HashSet<uint>? MapIds => [this.fishingSpotRow.Base.TerritoryType.ValueNullable?.Map.RowId ?? 0];
}