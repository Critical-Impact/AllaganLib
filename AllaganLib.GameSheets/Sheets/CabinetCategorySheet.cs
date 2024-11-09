using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class CabinetCategorySheet : ExtendedSheet<CabinetCategory, CabinetCategoryRow, CabinetCategorySheet>, IExtendedSheet
{
    public CabinetCategorySheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }
}
public class CabinetCategoryRow : ExtendedRow<CabinetCategory, CabinetCategoryRow, CabinetCategorySheet>;

