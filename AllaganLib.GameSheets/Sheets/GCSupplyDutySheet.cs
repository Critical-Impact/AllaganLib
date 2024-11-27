using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GCSupplyDutySheet : ExtendedSheet<GCSupplyDuty, GCSupplyDutyRow, GCSupplyDutySheet>, IExtendedSheet
{
    private GCSupplyDutyRewardSheet? gcSupplyDutyRewardSheet;

    public GCSupplyDutySheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
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

public class GCSupplyDutyRow : ExtendedRow<GCSupplyDuty, GCSupplyDutyRow, GCSupplyDutySheet>
{
    private GCSupplyDutyRewardRow? gcSupplyDutyRewardRow;

    public GCSupplyDutyRewardRow? GetGCSupplyDutyRewardRow()
    {
        if (this.gcSupplyDutyRewardRow == null)
        {
            return this.gcSupplyDutyRewardRow ??= this.Sheet.GetGCSupplyDutyRewardSheet().GetRow(this.RowId);
        }

        return this.gcSupplyDutyRewardRow;
    }
}