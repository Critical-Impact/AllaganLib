using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemExplorationVentureSource : ItemVentureSource
{
    public ItemExplorationVentureSource(ItemRow itemRow, RetainerTaskRow retainerTaskRow, ItemInfoType ventureType) : base(itemRow, retainerTaskRow, ventureType)
    {
    }
}