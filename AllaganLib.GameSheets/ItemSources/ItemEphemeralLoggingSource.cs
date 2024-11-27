using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemEphemeralLoggingSource : ItemGatheringSource
{
    public ItemEphemeralLoggingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.EphemeralLogging)
    {
    }
}