using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class FittingShopItemSetRow : ExtendedRow<FittingShopItemSet, FittingShopItemSetRow, FittingShopItemSetSheet>
{
    private List<ItemRow>? items;

    public List<ItemRow> Items
    {
        get
        {
            if (this.items == null)
            {
                var itemSheet = this.Sheet.SheetManager.GetSheet<ItemSheet>();
                uint[] itemIds = this.Base.Item.Select(c => c.RowId).ToArray();
                this.items = itemIds.Where(c => c != 0).Select(c => itemSheet.GetRowOrDefault(c)).Where(c => c != null)
                    .Select(c => c!).ToList();
            }

            return this.items;
        }
    }
}