using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ClassJobRow : ExtendedRow<ClassJob, ClassJobRow, ClassJobSheet>
{
    public int Icon => (int)(62000 + this.RowId);
}