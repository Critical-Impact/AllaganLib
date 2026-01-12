using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCompanyCraftRequirementSource : ItemSource
{
    public CompanyCraftSequenceRow CompanyCraftSequence { get; }

    public CompanyCraftMaterial CompanyCraftMaterial { get; }

    public ItemCompanyCraftRequirementSource(ItemRow item, ItemRow costItem, CompanyCraftMaterial companyCraftMaterial, CompanyCraftSequenceRow companyCraftSequence)
        : base(ItemInfoType.FreeCompanyCraftRecipe)
    {
        this.Item = item;
        this.CostItem = costItem;
        this.CompanyCraftSequence = companyCraftSequence;
        this.CompanyCraftMaterial = companyCraftMaterial;
    }

    public override uint Quantity => this.CompanyCraftMaterial.Quantity;

    public override RelationshipType RelationshipType => RelationshipType.CraftedFrom;

    public override RelationshipType? CostRelationshipType => RelationshipType.CraftedInto;
}