using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemEphemeralQuarryingSource : ItemGatheringSource
{
    public ItemEphemeralQuarryingSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.EphemeralQuarrying)
    {
    }
}