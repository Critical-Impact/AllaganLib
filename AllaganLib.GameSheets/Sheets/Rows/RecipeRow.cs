using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class RecipeRow : ExtendedRow<Recipe, RecipeRow, RecipeSheet>
{
    private CraftTypeRow? craftTypeRow;
    private RecipeLevelTableRow? recipeLevelTableRow;
    private Dictionary<uint, uint>? ingredientCounts;
    private ItemRow? itemResult;

    public ItemRow? ItemResult => this.itemResult ??= this.Sheet.GetItemSheet().GetRowOrDefault(this.Base.ItemResult.RowId);

    public CraftTypeRow? CraftType
    {
        get
        {
            return this.craftTypeRow ??= this.Sheet.GetCraftTypeSheet().GetRowOrDefault(this.Base.CraftType.RowId);
        }
    }

    public RecipeLevelTableRow? RecipeLevelTable
    {
        get
        {
            return this.recipeLevelTableRow ??=
                this.Sheet.GetRecipeLevelTableSheet().GetRowOrDefault(this.Base.RecipeLevelTable.RowId);
        }
    }

    public Dictionary<uint, uint> IngredientCounts
    {
        get
        {
            if (this.ingredientCounts == null)
            {
                var counts = new Dictionary<uint, uint>();
                for (var index = 0; index < this.Base.Ingredient.Count; index++)
                {
                    var ingredient = this.Base.Ingredient[index];
                    if (ingredient.RowId == uint.MaxValue)
                    {
                        // Handles a case where the ingredient's ID = -1;
                        continue;
                    }
                    var count = this.Base.AmountIngredient[index];
                    if (ingredient.RowId == 0 || count == 0)
                    {
                        continue;
                    }

                    counts.TryAdd(ingredient.RowId, 0);
                    counts[ingredient.RowId] += count;
                }

                this.ingredientCounts = counts;
            }

            return this.ingredientCounts;
        }
    }

    public uint? GetIngredientCount(uint itemId)
    {
        return this.IngredientCounts.TryGetValue(itemId, out var value) ? value : null;
    }



}