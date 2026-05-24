using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemFolkloreTomeSource : ItemSource
{
    private readonly List<ItemRow> unlockedItems;

    public ItemFolkloreTomeSource(ItemRow tomeItem, RowRef<NotebookDivision> notebookDivision, List<ItemRow> unlockedItems)
        : base(ItemInfoType.FolkloreTome)
    {
        this.Item = tomeItem;
        this.NotebookDivision = notebookDivision;
        this.unlockedItems = unlockedItems;
    }

    public RowRef<NotebookDivision> NotebookDivision { get; }

    public IReadOnlyList<ItemRow> UnlockedItems => this.unlockedItems;

    protected override IReadOnlyDictionary<RelatedItemKey, IReadOnlyList<ItemInfo>>? CreateRelatedItems()
    {
        if (this.unlockedItems.Count == 0)
        {
            return null;
        }

        var divisionName = this.NotebookDivision.ValueNullable?.Name.ExtractText() ?? "Folklore Tome";
        return new Dictionary<RelatedItemKey, IReadOnlyList<ItemInfo>>
        {
            [RelatedItemKey.Of(this.NotebookDivision.RowId.ToString(), divisionName, RelationshipType.CollectedFrom)]
                = this.unlockedItems.Select(i => ItemInfo.Create(i)).ToArray(),
        };
    }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.UsedIn;
}
