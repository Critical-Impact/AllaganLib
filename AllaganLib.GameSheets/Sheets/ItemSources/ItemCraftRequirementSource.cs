using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemCraftRequirementSource : ItemSource
{
    public RecipeRow Recipe { get; }

    public ItemCraftRequirementSource(ItemRow item, RecipeRow recipe)
        : base(ItemInfoType.CraftRecipe)
    {
        this.Item = item;
        this.Recipe = recipe;
    }

    public override uint Quantity => this.Recipe.GetIngredientCount(this.Item.RowId) ?? 0;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Recipe.CraftType!.Icon;
}