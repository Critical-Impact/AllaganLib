using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class TerritoryTypeSheet : ExtendedSheet<TerritoryType, TerritoryTypeRow, TerritoryTypeSheet>, IExtendedSheet
{
    private MapSheet? mapSheet;

    public TerritoryTypeSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public MapSheet GetMapSheet()
    {
        return this.mapSheet ??= this.SheetManager.GetSheet<MapSheet>();
    }

    public override void CalculateLookups()
    {
    }
}