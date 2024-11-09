using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GCScripShopCategoryRow : ExtendedRow<GCScripShopCategory, GCScripShopCategoryRow, GCScripShopCategorySheet>
{
    private GrandCompanyRow? grandCompany;

    public GrandCompanyRow GrandCompany
    {
        get { return this.grandCompany ??= this.Sheet.GetGrandCompanySheet().GetRow(this.Base.GrandCompany.RowId); }
    }
}