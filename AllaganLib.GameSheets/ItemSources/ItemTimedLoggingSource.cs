using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemTimedLoggingSource : ItemGatheringSource
{
    public ItemTimedLoggingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.TimedLogging)
    {
    }
}