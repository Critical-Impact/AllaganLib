using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GatheringTypeSheet : ExtendedSheet<GatheringType, GatheringTypeRow, GatheringTypeSheet>, IExtendedSheet
{
    private GatheringPointBaseSheet? gatheringPointBaseSheet;

    public GatheringTypeSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public GatheringPointBaseSheet GetGatheringPointBaseSheet()
    {
        return this.gatheringPointBaseSheet ??=
            this.SheetManager.GetSheet<GatheringPointBaseSheet>();
    }

    public override void CalculateLookups()
    {
    }
}