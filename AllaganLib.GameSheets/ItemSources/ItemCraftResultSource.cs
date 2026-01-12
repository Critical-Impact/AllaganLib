using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
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

    /// <inheritdoc/>
    protected override IReadOnlyList<ItemInfo> CreateCostItems()
    {
        List<ItemInfo> items = new List<ItemInfo>();
        foreach (var (itemId, quantity) in this.Recipe.IngredientCounts)
        {
            if (itemId == 0)
            {
                continue;
            }

            var itemRow = this.Item.Sheet.GetRowOrDefault(itemId);
            if (itemRow != null)
            {
                items.Add(ItemInfo.Create(itemRow, quantity));
            }
        }
        return items.ToArray();
    }

    public override uint Quantity => this.Recipe.Base.AmountResult;

    public override RelationshipType RelationshipType => RelationshipType.CraftedInto;

    public override RelationshipType? CostRelationshipType => RelationshipType.CraftedFrom;
}