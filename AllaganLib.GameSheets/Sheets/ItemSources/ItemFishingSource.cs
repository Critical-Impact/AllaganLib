using System.Collections.Generic;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

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

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;

    public FishingSpotRow FishingSpotRow => this.fishingSpotRow;

    public override HashSet<uint>? MapIds => [this.fishingSpotRow.Base.TerritoryType.ValueNullable?.Map.RowId ?? 0];
}