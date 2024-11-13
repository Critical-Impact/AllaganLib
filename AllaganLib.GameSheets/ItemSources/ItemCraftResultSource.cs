using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCraftResultSource : ItemSource
{
    public RecipeRow Recipe { get; }

    public ItemCraftResultSource(ItemRow item, RecipeRow recipe)
        : base(ItemInfoType.CraftRecipe)
    {
        this.Item = item;
        this.Recipe = recipe;
    }

    public override uint Quantity => this.Recipe.Base.AmountResult;
}