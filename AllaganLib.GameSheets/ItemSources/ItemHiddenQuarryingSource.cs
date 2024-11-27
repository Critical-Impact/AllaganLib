using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemHiddenQuarryingSource : ItemGatheringSource
{
    public ItemHiddenQuarryingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.HiddenQuarrying)
    {
    }
}