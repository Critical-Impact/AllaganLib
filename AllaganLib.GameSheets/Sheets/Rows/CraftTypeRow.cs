using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class CraftTypeRow : ExtendedRow<CraftType, CraftTypeRow, CraftTypeSheet>
{
    private string? formattedName;

    public string FormattedName => this.formattedName ??= this.Base.Name.ExtractText();

    public ushort Icon => (ushort)(this.RowId + 62502);
}