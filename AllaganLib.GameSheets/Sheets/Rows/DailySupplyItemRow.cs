using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class DailySupplyItemRow : ExtendedRow<DailySupplyItem, DailySupplyItemRow, DailySupplyItemSheet>
{
    private Dictionary<int, GCSupplyDutyRewardRow?>? gcSupplyDutyRewardRow;

    public GCSupplyDutyRewardRow? GetGCSupplyDutyRewardRow(int supplyIndex)
    {
        if (this.gcSupplyDutyRewardRow == null)
        {
            this.gcSupplyDutyRewardRow = new();
            for (var index = 0; index < this.Base.RecipeLevel.Count; index++)
            {
                var level = this.Base.RecipeLevel[index];
                if (level == 0)
                {
                    continue;
                }

                this.gcSupplyDutyRewardRow[index] = this.Sheet.GetGCSupplyDutyRewardSheet().GetRow(level);
            }
        }

        return this.gcSupplyDutyRewardRow.GetValueOrDefault(supplyIndex);
    }
}