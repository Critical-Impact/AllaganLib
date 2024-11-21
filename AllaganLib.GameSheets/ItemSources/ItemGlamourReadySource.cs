using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGlamourReadySource : ItemSource
{
    public List<ItemRow> SetItems { get; }

    public ItemGlamourReadySource(ItemRow itemRow, List<ItemRow> setItems)
        : base(ItemInfoType.GlamourReady)
    {
        this.SetItems = setItems;
        this.Item = itemRow;
    }

    public override uint Quantity => 0;
}