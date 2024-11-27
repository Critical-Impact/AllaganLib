using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemMiningSource : ItemGatheringSource
{
    public ItemMiningSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.Mining)
    {
    }
}