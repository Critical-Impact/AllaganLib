using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemEphemeralHarvestingSource : ItemGatheringSource
{
    public ItemEphemeralHarvestingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.EphemeralHarvesting)
    {
    }
}