using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;


namespace AllaganLib.GameSheets.Sheets.Rows;

public partial class ItemRow
{
    private List<GatheringTypeRow> GenerateGatheringTypes()
    {
        var gatheringTypeRows = new Dictionary<uint, GatheringTypeRow>();
        var gatheringTypeSheet = this.Sheet.SheetManager.GetSheet<GatheringTypeSheet>();
        foreach (var gatheringType in gatheringTypeSheet)
        {
            foreach (var basePoint in gatheringType.GatheringPointBases)
            {
                var gatheringItems = basePoint.GatheringItems;
                if (gatheringItems != null)
                {
                    foreach (var gatheringItemRow in gatheringItems)
                    {
                        if (gatheringItemRow.Base.Item.RowId == this.RowId)
                        {
                            gatheringTypeRows.TryAdd(gatheringItemRow.RowId, gatheringType);
                        }
                    }
                }
            }
        }

        return gatheringTypeRows.Select(c => c.Value).ToList();
    }
}