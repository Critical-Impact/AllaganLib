using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class FittingShopItemSetSheet : ExtendedSheet<FittingShopItemSet, FittingShopItemSetRow, FittingShopItemSetSheet>, IExtendedSheet
{
    public FittingShopItemSetSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }
}