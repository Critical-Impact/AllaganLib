using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemExplorationVentureSource : ItemVentureSource
{
    public ItemExplorationVentureSource(ItemRow itemRow, RetainerTaskRow retainerTaskRow, ItemInfoType ventureType) : base(itemRow, retainerTaskRow, ventureType)
    {
    }
}