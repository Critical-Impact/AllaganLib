using System.Collections.Generic;
using System.Linq;
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

    public override List<ItemRow> CostItems
    {
        get
        {
            return this.Recipe.IngredientCounts.Select(c => this.Item.Sheet.GetRow(c.Key)).ToList();
        }
    }

    public override uint Quantity => this.Recipe.Base.AmountResult;
}