using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemSupplementSource : ItemSource
{
    protected ItemSupplementSource(ItemRow item, ItemRow costItem, ItemInfoType infoType) : base(infoType)
    {
        this.Item = item;
        this.CostItem = costItem;
    }

    public override uint Quantity => 1;

}

public class ItemDesynthSource(ItemRow item, ItemRow costItem)
    : ItemSupplementSource(item, costItem, ItemInfoType.Desynthesis);

public class ItemReductionSource(ItemRow item, ItemRow costItem)
    : ItemSupplementSource(item, costItem, ItemInfoType.Reduction);

public class ItemLootSource(ItemRow item, ItemRow costItem)
    : ItemSupplementSource(item, costItem, ItemInfoType.Loot);

public class ItemGardeningSource(ItemRow item, ItemRow costItem)
    : ItemSupplementSource(item, costItem, ItemInfoType.Gardening);