using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class CraftTypeSheet : ExtendedSheet<CraftType, CraftTypeRow, CraftTypeSheet>, IExtendedSheet
{
    public CraftTypeSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(
        gameData,
        sheetManager,
        sheetIndexer,
        itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }
}