using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemQuickVentureSource : ItemVentureSource
{
    public ItemQuickVentureSource(ItemRow itemRow, RetainerTaskRow retainerTaskRow)
        : base(itemRow, retainerTaskRow, ItemInfoType.QuickVenture)
    {
    }
}