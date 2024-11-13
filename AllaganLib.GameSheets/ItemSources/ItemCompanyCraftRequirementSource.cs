using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Helpers;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCompanyCraftRequirementSource : ItemSource
{
    public RecipeRow Recipe { get; }

    public CompanyCraftSequenceRow CompanyCraftSequence { get; }

    public CompanyCraftMaterial CompanyCraftMaterial { get; }

    public ItemCompanyCraftRequirementSource(ItemRow item, CompanyCraftMaterial companyCraftMaterial, CompanyCraftSequenceRow companyCraftSequence)
        : base(ItemInfoType.FreeCompanyCraftRecipe)
    {
        this.Item = item;
        this.CompanyCraftSequence = companyCraftSequence;
        this.CompanyCraftMaterial = companyCraftMaterial;
    }

    public override uint Quantity => this.CompanyCraftMaterial.Quantity;
}