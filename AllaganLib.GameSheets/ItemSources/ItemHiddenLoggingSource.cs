using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemHiddenLoggingSource : ItemGatheringSource
{
    public ItemHiddenLoggingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.HiddenLogging)
    {
    }
}