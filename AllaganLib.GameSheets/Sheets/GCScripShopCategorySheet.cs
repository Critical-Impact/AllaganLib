using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GCScripShopCategorySheet : ExtendedSheet<GCScripShopCategory, GCScripShopCategoryRow, GCScripShopCategorySheet>, IExtendedSheet
{
    private Dictionary<uint, List<uint>>? byGrandCompanyId;
    private Dictionary<uint, List<GCScripShopCategoryRow>>? byGrandCompany;
    private GrandCompanySheet? grandCompanySheet;

    public GCScripShopCategorySheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public GrandCompanySheet GetGrandCompanySheet()
    {
        return this.grandCompanySheet ??= this.SheetManager.GetSheet<GrandCompanySheet>();
    }

    public List<GCScripShopCategoryRow> GetCategoriesByGrandCompanyId(uint grandCompanyId)
    {
        this.byGrandCompany = this.SheetIndexer.OneToMany<GCScripShopCategory, GCScripShopCategoryRow, GCScripShopCategorySheet, GrandCompany, GrandCompanyRow, GrandCompanySheet>(this, row => row.GrandCompany);
        if (this.byGrandCompany.TryGetValue(grandCompanyId, out var value))
        {
            return value;
        }

        return [];
    }

    public override void CalculateLookups()
    {
    }
}