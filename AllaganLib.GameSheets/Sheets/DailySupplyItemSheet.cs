using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class DailySupplyItemSheet : ExtendedSheet<DailySupplyItem, DailySupplyItemRow, DailySupplyItemSheet>, IExtendedSheet
{
    public DailySupplyItemSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }
}