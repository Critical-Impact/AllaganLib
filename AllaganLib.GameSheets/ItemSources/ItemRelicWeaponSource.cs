using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemZodiacWeaponSource : ItemSource
{
    public ClassJobRow ClassJob { get; }

    public ItemRow Zodiac;
    public ItemRow ZenithZodiac;
    public ItemRow AtmaZodiac;
    public ItemRow AnimusZodiac;
    public ItemRow NovusZodiac;
    public ItemRow NexusZodiac;
    public ItemRow ZodiacBraves;
    public ItemRow ZodiacZeta;
    public List<ItemRow> Items;

    public ItemZodiacWeaponSource(ItemRow item, ClassJobRow classJob, List<ItemRow> relatedItems)
        : base(ItemInfoType.ZodiacWeapon)
    {
        this.ClassJob = classJob;
        this.Item = item;
        this.Items = relatedItems;
        this.Zodiac = relatedItems[0];
        this.ZenithZodiac = relatedItems[1];
        this.AtmaZodiac = relatedItems[2];
        this.AnimusZodiac = relatedItems[3];
        this.NovusZodiac = relatedItems[4];
        this.NexusZodiac = relatedItems[5];
        this.ZodiacBraves = relatedItems[6];
        this.ZodiacZeta = relatedItems[7];
    }

    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        return this.Items.Select(c => ItemInfo.Create(c)).ToList();
    }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.RelatedTo;
}