using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class
    CompanyCraftSupplyItemRow : ExtendedRow<CompanyCraftSupplyItem, CompanyCraftSupplyItemRow,
    CompanyCraftSupplyItemSheet>
{
    private ItemRow? item;

    public ItemRow Item
    {
        get
        {
            if (this.item == null)
            {
                var itemSheet = this.Sheet.GetItemSheet();
                this.item = itemSheet.GetRow(this.Base.Item.RowId);
            }

            return this.item;
        }
    }
}