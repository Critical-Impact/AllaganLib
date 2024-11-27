using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemHiddenHarvestingSource : ItemGatheringSource
{
    public ItemHiddenHarvestingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.HiddenHarvesting)
    {
    }
}