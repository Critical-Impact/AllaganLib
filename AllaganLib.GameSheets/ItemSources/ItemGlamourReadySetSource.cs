using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGlamourReadySetSource : ItemSource
{
    public List<ItemRow> SetItems { get; }

    public ItemGlamourReadySetSource(ItemRow itemRow, List<ItemRow> setItems)
        : base(ItemInfoType.GlamourReadySet)
    {
        this.SetItems = setItems;
        this.Item = itemRow;
    }

    protected override IReadOnlyDictionary<RelatedItemKey, IReadOnlyList<ItemInfo>>? CreateRelatedItems()
    {
        return new Dictionary<RelatedItemKey, IReadOnlyList<ItemInfo>>
        {
            [RelatedItemKey.Of("setItems", "Set Items", RelationshipType.InSet)] = this.SetItems.Select(c => ItemInfo.Create(c)).ToArray(),
        };
    }

    public override uint Quantity => 0;

    public override RelationshipType RelationshipType => RelationshipType.UsedIn;
}