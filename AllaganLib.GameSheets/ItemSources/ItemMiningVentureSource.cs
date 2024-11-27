using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemMiningVentureSource : ItemVentureSource
{
    public ItemMiningVentureSource(ItemRow itemRow, ItemRow venture, RetainerTaskRow retainerTaskRow)
        : base(itemRow, venture, retainerTaskRow, ItemInfoType.MiningVenture)
    {
    }
}