using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class InclusionShopCategorySheet : ExtendedSheet<InclusionShopCategory, InclusionShopCategoryRow, InclusionShopCategorySheet>, IExtendedSheet
{
    private InclusionShopSheet? inclusionShopSheet = null;
    private Dictionary<uint, InclusionShopCategoryRow>? seriesToInclusionShopCategories = null;

    public InclusionShopCategorySheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public InclusionShopSheet GetInclusionShopSheet()
    {
        return this.inclusionShopSheet ??= this.SheetManager.GetSheet<InclusionShopSheet>();
    }

    public InclusionShopCategoryRow? GetInclusionShopCategoryBySeries(uint seriesId)
    {
        this.seriesToInclusionShopCategories ??= this.Where(c => c.Base.InclusionShopSeries.RowId != 0)
            .ToDictionary(c => c.Base.InclusionShopSeries.RowId, c => c);
        return this.seriesToInclusionShopCategories.GetValueOrDefault(seriesId);
    }

    public override void CalculateLookups()
    {
    }
}