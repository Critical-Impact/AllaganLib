using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class InclusionShopCategoryRow : ExtendedRow<InclusionShopCategory, InclusionShopCategoryRow, InclusionShopCategorySheet>
{
    private HashSet<InclusionShopRow>? inclusionShops;

    public HashSet<InclusionShopRow> InclusionShops => this.inclusionShops ??= this.Sheet.GetInclusionShopSheet().GetInclusionShopsByCategoryId(this.RowId) ?? new HashSet<InclusionShopRow>();
}