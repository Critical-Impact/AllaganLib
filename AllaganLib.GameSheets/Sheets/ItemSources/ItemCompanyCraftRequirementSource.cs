using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Helpers;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

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

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Recipe.CraftType!.Icon;
}