using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class TerritoryTypeRow : ExtendedRow<TerritoryType, TerritoryTypeRow, TerritoryTypeSheet>
{
    private MapRow? mapRow;

    public MapRow? Map
    {
        get
        {
            return this.mapRow ??= this.Sheet.GetMapSheet().GetRowOrDefault(this.Base.Map.RowId);
        }
    }
}