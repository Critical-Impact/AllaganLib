using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SpearfishingNotebookRow : ExtendedRow<SpearfishingNotebook, SpearfishingNotebookRow, SpearfishingNotebookSheet>
{
    private TerritoryTypeRow? territoryType;

    public TerritoryTypeRow? TerritoryType
    {
        get
        {
            return this.territoryType ??= this.Base.TerritoryType.IsValid ? this.Sheet.GetTerritoryTypeSheet().GetRow(this.Base.TerritoryType.RowId) : null;
        }
    }
}