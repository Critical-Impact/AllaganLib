using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCalamitySalvagerShopSource : ItemGilShopSource
{
    public ItemCalamitySalvagerShopSource(GilShopItemRow gilShopItem, GilShopRow gilShop)
        : base(gilShopItem, gilShop, ItemInfoType.CalamitySalvagerShop)
    {
    }
}