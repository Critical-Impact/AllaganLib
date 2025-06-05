using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemSupplementSource : ItemSource
{
    public ItemSupplement Supplement { get; }

    protected ItemSupplementSource(ItemRow item, ItemRow costItem, ItemSupplement supplement, ItemInfoType infoType) : base(infoType)
    {
        this.Item = item;
        this.CostItem = costItem;
        this.Supplement = supplement;
    }

    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        return [ItemInfo.Create(this.Item, min: this.Supplement.Min, max: this.Supplement.Max, probability: this.Supplement.Probability)];
    }

    protected override IReadOnlyList<ItemInfo>? CreateCostItems()
    {
        return [ItemInfo.Create(this.CostItem)];
    }

    public override uint Quantity => 1;

}

public class ItemDesynthSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Desynthesis);

public class ItemReductionSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Reduction);

public class ItemLootSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Loot);

public class ItemGardeningSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Gardening);

public class ItemCardPackSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.CardPack);

public class ItemCofferSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Coffer);

public class ItemPalaceOfTheDeadSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.PalaceOfTheDead);

public class ItemHeavenOnHighSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.HeavenOnHigh);

public class ItemEurekaOrthosSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.EurekaOrthos);

public class ItemAnemosSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Anemos);

public class ItemPagosSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Pagos);

public class ItemPyrosSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Pyros);

public class ItemHydatosSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Hydatos);

public class ItemBozjaSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Bozja);

public class ItemLogogramSource(ItemRow item, ItemRow costItem, ItemSupplement supplement)
    : ItemSupplementSource(item, costItem, supplement, ItemInfoType.Logogram);