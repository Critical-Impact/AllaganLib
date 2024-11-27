using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemTimedMiningSource : ItemGatheringSource
{
    public ItemTimedMiningSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.TimedMining)
    {
    }
}