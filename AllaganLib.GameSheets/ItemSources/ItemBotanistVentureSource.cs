using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemBotanistVentureSource : ItemVentureSource
{
    public ItemBotanistVentureSource(ItemRow itemRow, ItemRow venture, RetainerTaskRow retainerTaskRow)
        : base(itemRow, venture, retainerTaskRow, ItemInfoType.BotanyVenture)
    {
    }
}