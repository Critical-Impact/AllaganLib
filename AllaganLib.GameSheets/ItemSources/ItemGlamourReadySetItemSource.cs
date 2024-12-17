using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGlamourReadySetItemSource : ItemSource
{
    public List<ItemRow> SetItems { get; }

    public ItemRow ConvertedItem { get; }

    public ItemGlamourReadySetItemSource(ItemRow itemRow, ItemRow convertedItem, List<ItemRow> setItems)
        : base(ItemInfoType.GlamourReadySetItem)
    {
        this.ConvertedItem = convertedItem;
        this.SetItems = setItems;
        this.Item = itemRow;
    }

    public override uint Quantity => 0;
}