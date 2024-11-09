using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Caches;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public abstract class ItemShopSource : ItemSource
{
    public IShop Shop { get; }

    protected ItemShopSource(IShop shop, ItemInfoType infoType) : base(infoType)
    {
        this.Shop = shop;
    }
}