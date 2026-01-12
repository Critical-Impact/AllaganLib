using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemAnimaWeaponSource : ItemSource
{
    public ClassJobRow ClassJob { get; }

    public ItemRow Animated;
    public ItemRow Awoken;
    public ItemRow Anima;
    public ItemRow Hyperconductive;
    public ItemRow Reconditioned;
    public ItemRow Sharpened;
    public ItemRow Complete;
    public ItemRow Lux;
    public List<ItemRow> Items;

    public ItemAnimaWeaponSource(ItemRow item, ClassJobRow classJob, List<ItemRow> relatedItems)
        : base(ItemInfoType.AnimaWeapon)
    {
        this.ClassJob = classJob;
        this.Item = item;
        this.Items = relatedItems;
        this.Animated = relatedItems[0];
        this.Awoken = relatedItems[1];
        this.Anima = relatedItems[2];
        this.Hyperconductive = relatedItems[3];
        this.Reconditioned = relatedItems[4];
        this.Sharpened = relatedItems[5];
        this.Complete = relatedItems[6];
        this.Lux = relatedItems[7];
    }

    public override RelationshipType RelationshipType => RelationshipType.RelatedTo;

    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        return this.Items.Select(c => ItemInfo.Create(c)).ToList();
    }

    public override uint Quantity => 1;
}