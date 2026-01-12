using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCraftRequirementSource : ItemSource
{
    public RecipeRow Recipe { get; }

    public ItemCraftRequirementSource(ItemRow item, ItemRow ingredient, RecipeRow recipe)
        : base(ItemInfoType.CraftRecipe)
    {
        this.Item = item;
        this.CostItem = ingredient;
        this.Recipe = recipe;
    }

    public override uint Quantity => this.Recipe.GetIngredientCount(this.Item.RowId) ?? 0;

    public override RelationshipType RelationshipType => RelationshipType.CraftedFrom;

    public override RelationshipType? CostRelationshipType => RelationshipType.CraftedInto;
}