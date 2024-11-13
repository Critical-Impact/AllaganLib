using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

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
}