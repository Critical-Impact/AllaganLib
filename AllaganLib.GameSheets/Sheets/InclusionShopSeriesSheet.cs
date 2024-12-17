using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class InclusionShopSeriesSheet : ExtendedSubrowSheet<InclusionShopSeries, InclusionShopSeriesRow, InclusionShopSeriesSheet>, IExtendedSheet
{
    private InclusionShopCategorySheet? inclusionShopCategorySheet;

    public InclusionShopSeriesSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer)
        : base(gameData, sheetManager, sheetIndexer)
    {
    }

    public InclusionShopCategorySheet GetInclusionShopCategorySheet()
    {
        return this.inclusionShopCategorySheet ??= this.SheetManager.GetSheet<InclusionShopCategorySheet>();
    }


    public override void CalculateLookups()
    {

    }
}