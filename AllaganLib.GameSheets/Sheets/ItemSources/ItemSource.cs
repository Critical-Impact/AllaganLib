using System.Collections.Generic;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public abstract class ItemSource : ItemInfo
{
    public ItemRow Item { get; protected set; }

    public ItemRow? CostItem { get; protected set; }

    public virtual List<ItemRow> Items => [this.Item];

    public virtual List<ItemRow> CostItems => this.CostItem != null ? [this.CostItem] : [];

    public override HashSet<uint>? MapIds => null;

    public ItemSource(ItemInfoType infoType)
    {
        this.Type = infoType;
    }
}