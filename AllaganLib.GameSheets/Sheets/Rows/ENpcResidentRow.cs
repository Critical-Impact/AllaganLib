using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ENpcResidentRow : ExtendedRow<ENpcResident, ENpcResidentRow, ENpcResidentSheet>
{
    private ENpcBaseRow? baseRow;

    public ENpcBaseRow ENpcBase
    {
        get { return this.baseRow ??= this.Sheet.GetENpcBaseSheet().GetRow(this.RowId); }
    }
}