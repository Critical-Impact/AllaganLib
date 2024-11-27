using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemEphemeralMiningSource : ItemGatheringSource
{
    public ItemEphemeralMiningSource(GatheringItemRow gatheringItem)
        : base(gatheringItem, ItemInfoType.EphemeralMining)
    {
    }
}