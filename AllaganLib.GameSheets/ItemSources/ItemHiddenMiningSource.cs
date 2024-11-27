using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemHiddenMiningSource : ItemGatheringSource
{
    public ItemHiddenMiningSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.HiddenMining)
    {
    }
}