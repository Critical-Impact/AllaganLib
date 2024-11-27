using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemWoodlandExplorationVentureSource : ItemVentureSource
{
    public ItemWoodlandExplorationVentureSource(ItemRow itemRow, ItemRow venture, RetainerTaskRow retainerTaskRow)
        : base(itemRow, venture, retainerTaskRow, ItemInfoType.BotanyExplorationVenture)
    {
    }
}