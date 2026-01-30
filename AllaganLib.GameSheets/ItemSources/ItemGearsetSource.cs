using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGearsetSource : ItemSource
{
    private readonly List<ItemRow> setItems;
    private readonly Gearset gearset;

    public ItemGearsetSource(ItemRow itemRow, List<ItemRow> setItems, Gearset gearset) : base(ItemInfoType.Gearset)
    {
        this.setItems = setItems;
        this.gearset = gearset;
        this.Item = itemRow;
    }

    protected override IReadOnlyDictionary<RelatedItemKey, IReadOnlyList<ItemInfo>>? CreateRelatedItems()
    {
        var itemSheet = this.Item.Sheet;
        return new Dictionary<RelatedItemKey, IReadOnlyList<ItemInfo>>
        {
            [RelatedItemKey.Of(this.gearset.Key, this.gearset.Name, RelationshipType.InSet)] = this.SetItems.Select(c => ItemInfo.Create(itemSheet.GetRow(c.RowId))).ToArray(),
        };
    }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.InSet;

    public List<ItemRow> SetItems => this.setItems;

    public Gearset Gearset => this.gearset;
}