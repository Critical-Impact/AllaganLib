using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemZodiacWeaponSource : ItemRelicWeaponSource
{
    public ItemZodiacWeaponSource(ItemRow item, RelicWeapon relicWeapon, List<RelicWeapon> forms, List<ItemRow> relatedItems)
        : base(item, relicWeapon, forms, relatedItems, ItemInfoType.ZodiacWeapon)
    {
    }
}

public class ItemAnimaWeaponSource : ItemRelicWeaponSource
{
    public ItemAnimaWeaponSource(ItemRow item, RelicWeapon relicWeapon, List<RelicWeapon> forms, List<ItemRow> relatedItems)
        : base(item, relicWeapon, forms, relatedItems, ItemInfoType.AnimaWeapon)
    {
    }
}

public class ItemEurekanWeaponSource : ItemRelicWeaponSource
{
    public ItemEurekanWeaponSource(ItemRow item, RelicWeapon relicWeapon, List<RelicWeapon> forms, List<ItemRow> relatedItems)
        : base(item, relicWeapon, forms, relatedItems, ItemInfoType.EurekanWeapon)
    {
    }
}

public class ItemResistanceWeaponSource : ItemRelicWeaponSource
{
    public ItemResistanceWeaponSource(ItemRow item, RelicWeapon relicWeapon, List<RelicWeapon> forms, List<ItemRow> relatedItems)
        : base(item, relicWeapon, forms, relatedItems, ItemInfoType.ResistanceWeapon)
    {
    }
}

public class ItemMandervilleWeaponSource : ItemRelicWeaponSource
{
    public ItemMandervilleWeaponSource(ItemRow item, RelicWeapon relicWeapon, List<RelicWeapon> forms, List<ItemRow> relatedItems)
        : base(item, relicWeapon, forms, relatedItems, ItemInfoType.MandervilleWeapon)
    {
    }
}

public class ItemPhantomWeaponSource : ItemRelicWeaponSource
{
    public ItemPhantomWeaponSource(ItemRow item, RelicWeapon relicWeapon, List<RelicWeapon> forms, List<ItemRow> relatedItems)
        : base(item, relicWeapon, forms, relatedItems, ItemInfoType.PhantomWeapon)
    {
    }
}

public abstract class ItemRelicWeaponSource : ItemSource
{
    public RelicWeapon RelicWeapon { get; }

    public List<RelicWeapon> Forms { get; }

    public List<ItemRow> Items;

    public ItemRelicWeaponSource(ItemRow item, RelicWeapon relicWeapon, List<RelicWeapon> forms, List<ItemRow> relatedItems, ItemInfoType itemInfoType)
        : base(itemInfoType)
    {
        this.RelicWeapon = relicWeapon;
        this.Forms = forms;
        this.Item = item;
        this.Items = relatedItems;
    }

    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        return this.Items.Select(c => ItemInfo.Create(c)).ToList();
    }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.RelatedTo;
}