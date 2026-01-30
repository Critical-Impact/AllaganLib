using AllaganLib.GameSheets.Model;
using AllaganLib.Shared.Extensions;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ENpcResidentRow : ExtendedRow<ENpcResident, ENpcResidentRow, ENpcResidentSheet>, INamed
{
    private ENpcBaseRow? baseRow;

    public ENpcBaseRow ENpcBase
    {
        get { return this.baseRow ??= this.Sheet.GetENpcBaseSheet().GetRow(this.RowId); }
    }

    public string Name => this.Base.Singular.ToImGuiString();
}