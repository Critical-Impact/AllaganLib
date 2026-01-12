using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCompanyCraftResultSource : ItemSource
{
    public CompanyCraftSequenceRow CompanyCraftSequence { get; }

    public ItemCompanyCraftResultSource(ItemRow item, CompanyCraftSequenceRow companyCraftSequence)
        : base(ItemInfoType.FreeCompanyCraftRecipe)
    {
        this.Item = item;
        this.CompanyCraftSequence = companyCraftSequence;
    }

    public override uint Quantity => 1;

    protected override IReadOnlyList<ItemInfo>? CreateCostItems()
    {
        var itemInfos = new List<ItemInfo>();
        foreach (var material in this.CompanyCraftSequence.MaterialsRequired(null))
        {
            if (material.ItemId == 0)
            {
                continue;
            }

            var item = this.Item.Sheet.GetRowOrDefault(material.ItemId);
            if (item != null)
            {
                itemInfos.Add(ItemInfo.Create(item, material.Quantity));
            }
        }
        return itemInfos;
    }

    public override RelationshipType RelationshipType => RelationshipType.CraftedInto;

    public override RelationshipType? CostRelationshipType => RelationshipType.CraftedFrom;
}