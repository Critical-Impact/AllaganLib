using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemLoggingSource : ItemGatheringSource
{
    public ItemLoggingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.Logging)
    {
    }
}