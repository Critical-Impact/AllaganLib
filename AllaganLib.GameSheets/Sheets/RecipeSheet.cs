using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class RecipeSheet : ExtendedSheet<Recipe, RecipeRow, RecipeSheet>, IExtendedSheet
{
    private CraftTypeSheet? craftTypeSheet;
    private ItemSheet? itemSheet;
    private RecipeLevelTableSheet? recipeLevelTableSheet;
    private Dictionary<uint, List<RecipeRow>>? recipesByItemId;
    private Dictionary<uint, List<RecipeRow>>? recipesByIngredientItemId;

    public RecipeSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public CraftTypeSheet GetCraftTypeSheet()
    {
        return this.craftTypeSheet ??= this.SheetManager.GetSheet<CraftTypeSheet>();
    }

    public RecipeLevelTableSheet GetRecipeLevelTableSheet()
    {
        return this.recipeLevelTableSheet ??= this.SheetManager.GetSheet<RecipeLevelTableSheet>();
    }

    public bool HasRecipesByItemId(uint itemId)
    {
        return this.GetRecipesByItemId().ContainsKey(itemId);
    }

    public List<RecipeRow>? GetRecipesByItemId(uint itemId)
    {
        return this.GetRecipesByItemId().GetValueOrDefault(itemId);
    }

    public Dictionary<uint, List<RecipeRow>> GetRecipesByItemId()
    {
        return this.recipesByItemId ??= this.SheetIndexer.ManyToOneId(
            this,
            row =>
            {
                return row.Base.ItemResult.RowId;
            });
    }

    public List<RecipeRow>? GetRecipesByIngredientItemId(uint itemId)
    {
        return this.GetRecipesByIngredientItemId().GetValueOrDefault(itemId);
    }

    public Dictionary<uint, List<RecipeRow>> GetRecipesByIngredientItemId()
    {
        return this.recipesByIngredientItemId ??= this.SheetIndexer.ManyToOneId(
            this,
            row =>
            {
                return row.Base.Ingredient.Select(c => c.RowId);
            });
    }

    public override void CalculateLookups()
    {
    }
}