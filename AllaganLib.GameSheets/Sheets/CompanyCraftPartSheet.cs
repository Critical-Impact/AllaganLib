using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class CompanyCraftPartSheet : ExtendedSheet<CompanyCraftPart, CompanyCraftPartRow, CompanyCraftPartSheet>, IExtendedSheet
{
    private CompanyCraftProcessSheet? companyCraftProcessSheet;

    public CompanyCraftPartSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }

    public CompanyCraftProcessSheet GetCompanyCraftProcessSheet()
    {
        return this.companyCraftProcessSheet ??=
            this.SheetManager.GetSheet<CompanyCraftProcessSheet>();
    }
}