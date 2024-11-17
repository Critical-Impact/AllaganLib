using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemExplorationVentureSource : ItemVentureSource
{
    public ItemExplorationVentureSource(ItemRow itemRow, ItemRow venture, RetainerTaskRow retainerTaskRow, ItemInfoType ventureType)
        : base(itemRow, venture, retainerTaskRow, ventureType)
    {
    }
}