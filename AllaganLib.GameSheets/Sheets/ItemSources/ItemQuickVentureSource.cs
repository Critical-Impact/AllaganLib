using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemQuickVentureSource : ItemVentureSource
{
    public ItemQuickVentureSource(ItemRow itemRow, RetainerTaskRow retainerTaskRow)
        : base(itemRow, retainerTaskRow, ItemInfoType.QuickVenture)
    {
    }
}