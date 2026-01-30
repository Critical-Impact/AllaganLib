using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class FishParameterSheet : ExtendedSheet<FishParameter, FishParameterRow, FishParameterSheet>, IExtendedSheet
{
    private FishingSpotSheet? fishingSpotSheet;

    public FishParameterSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public FishingSpotSheet FishingSpotSheet => this.fishingSpotSheet ??= this.SheetManager.GetSheet<FishingSpotSheet>();

    public override void CalculateLookups()
    {
    }
}