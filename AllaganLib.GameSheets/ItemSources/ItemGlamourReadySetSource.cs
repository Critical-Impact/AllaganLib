using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
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
    
    public override uint Quantity => 0;
}