using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemQuarryingSource : ItemGatheringSource
{
    public ItemQuarryingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.Quarrying)
    {
    }
}