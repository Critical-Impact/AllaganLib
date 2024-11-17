using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class DailySupplyItemSheet : ExtendedSheet<DailySupplyItem, DailySupplyItemRow, DailySupplyItemSheet>, IExtendedSheet
{
    private GCSupplyDutyRewardSheet? gcSupplyDutyRewardSheet;
    public DailySupplyItemSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public GCSupplyDutyRewardSheet GetGCSupplyDutyRewardSheet()
    {
        return this.gcSupplyDutyRewardSheet ??= this.SheetManager.GetSheet<GCSupplyDutyRewardSheet>();
    }

    public override void CalculateLookups()
    {
    }
}