using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemFishingSource : ItemSource
{
    public FishParameterRow FishParameter { get; }

    public List<FishingSpotRow> FishingSpots { get; }

    public ItemFishingSource(FishParameterRow fishParameter, List<FishingSpotRow> fishingSpots, ItemRow itemRow)
        : base(ItemInfoType.Fishing)
    {
        this.FishParameter = fishParameter;
        this.FishingSpots = fishingSpots;
        this.Item = itemRow!;
    }

    public override uint Quantity => 1;


    public override HashSet<uint>? MapIds => this.FishingSpots.Where(c => c.TerritoryType.RowId != 0).Select(c => c.Base.TerritoryType.ValueNullable?.Map.RowId ?? 0).Where(c => c != 0).Distinct().ToHashSet();

    public override RelationshipType RelationshipType => RelationshipType.CollectedFrom;
}