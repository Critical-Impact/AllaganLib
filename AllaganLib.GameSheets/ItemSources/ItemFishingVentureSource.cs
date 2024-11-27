using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemFishingVentureSource : ItemVentureSource
{
    public ItemFishingVentureSource(ItemRow itemRow, ItemRow venture, RetainerTaskRow retainerTaskRow)
        : base(itemRow, venture, retainerTaskRow, ItemInfoType.FishingVenture)
    {
    }
}