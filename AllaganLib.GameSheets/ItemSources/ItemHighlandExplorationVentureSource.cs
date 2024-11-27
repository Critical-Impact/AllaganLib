using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemHighlandExplorationVentureSource : ItemVentureSource
{
    public ItemHighlandExplorationVentureSource(ItemRow itemRow, ItemRow venture, RetainerTaskRow retainerTaskRow)
        : base(itemRow, venture, retainerTaskRow, ItemInfoType.MiningExplorationVenture)
    {
    }
}