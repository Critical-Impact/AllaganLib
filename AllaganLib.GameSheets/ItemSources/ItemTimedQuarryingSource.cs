using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemTimedQuarryingSource : ItemGatheringSource
{
    public ItemTimedQuarryingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.TimedQuarrying)
    {
    }
}