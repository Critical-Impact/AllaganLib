using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public sealed class ItemSecretRecipeBookUse : ItemSource
{
    private readonly List<RecipeRow> recipes;

    public ItemSecretRecipeBookUse(RowRef<SecretRecipeBook> secretRecipeBook, ItemRow bookItem, List<RecipeRow> recipes)
        : base(ItemInfoType.SecretRecipeBook)
    {
        this.Item = bookItem;
        this.SecretRecipeBook = secretRecipeBook;
        this.recipes = recipes;
    }

    /// <summary>
    /// Gets the related secret recipe book.
    /// </summary>
    public RowRef<SecretRecipeBook> SecretRecipeBook { get; }

    /// <summary>
    /// Gets the recipes unlocked by this book.
    /// </summary>
    public IReadOnlyList<RecipeRow> Recipes => this.recipes;

    protected override IReadOnlyDictionary<RelatedItemKey, IReadOnlyList<ItemInfo>>? CreateRelatedItems()
    {
        if (this.recipes.Count == 0)
        {
            return null;
        }

        var bookName = this.SecretRecipeBook.ValueNullable?.Name.ExtractText() ?? "Secret Recipe Book";
        var craftableItems = this.recipes
            .Select(r => r.ItemResult)
            .Where(i => i != null)
            .Select(i => ItemInfo.Create(i!))
            .ToArray();

        if (craftableItems.Length == 0)
        {
            return null;
        }

        return new Dictionary<RelatedItemKey, IReadOnlyList<ItemInfo>>
        {
            [RelatedItemKey.Of(this.SecretRecipeBook.RowId.ToString(), bookName, RelationshipType.CollectedFrom)]
                = craftableItems,
        };
    }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.UsedIn;
}