using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class InclusionShopSeriesRow :
    ExtendedSubrow<InclusionShopSeries, InclusionShopSeriesRow, InclusionShopSeriesSheet>
{
    public InclusionShopCategoryRow? Category => this.Sheet.GetInclusionShopCategorySheet().GetInclusionShopCategoryBySeries(this.RowId);
}