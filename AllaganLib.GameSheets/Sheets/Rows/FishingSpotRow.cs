using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class FishingSpotRow : ExtendedRow<FishingSpot, FishingSpotRow, FishingSpotSheet>
{
    private List<ItemRow>? itemRows;
    
    public List<ItemRow> Items
    {
        get
        {
            return this.itemRows ??= this.Base.Item.Where(c => c.IsValid).Select(c => this.Sheet.GetItemSheet().GetRow(c.RowId)).ToList();
        }
    }
}