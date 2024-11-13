using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class CabinetRow : ExtendedRow<Cabinet, CabinetRow, CabinetSheet>
{
    private CabinetCategoryRow? cabinetCategory;
    
    public CabinetCategoryRow? CabinetCategory => this.cabinetCategory ??= this.Sheet.CabinetCategorySheet.GetRowOrDefault(this.Base.Category.RowId);
}