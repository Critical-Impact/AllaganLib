using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemTimedHarvestingSource : ItemGatheringSource
{
    public ItemTimedHarvestingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.TimedHarvesting)
    {
    }
}