using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemHarvestingSource : ItemGatheringSource
{
    public ItemHarvestingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.Harvesting)
    {
    }
}