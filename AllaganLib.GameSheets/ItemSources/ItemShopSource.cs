using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemShopSource : ItemSource
{
    public IShop Shop { get; }

    protected ItemShopSource(IShop shop, ItemInfoType infoType) : base(infoType)
    {
        this.Shop = shop;
    }

    public override RelationshipType RelationshipType => RelationshipType.Purchaseable;

    public override RelationshipType? CostRelationshipType => RelationshipType.PurchasedWith;
}