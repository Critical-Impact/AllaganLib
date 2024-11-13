using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class HWDGathererInspectionRow : ExtendedRow<HWDGathererInspection, HWDGathererInspectionRow, HWDGathererInspectionSheet>
{
    private Dictionary<uint, List<HWDGathererInspection.HWDGathererInspectionDataStruct>>? resultsByItemReceived;
    private Dictionary<uint, List<HWDGathererInspection.HWDGathererInspectionDataStruct>>? resultsByGatheringItem;

    public List<HWDGathererInspection.HWDGathererInspectionDataStruct>? GetResultsByItemReceived(uint itemId)
    {
        this.resultsByItemReceived ??= this.Base.HWDGathererInspectionData.Where(c => c.ItemReceived.RowId != 0)
            .GroupBy(c => c.ItemReceived.RowId).ToDictionary(c => c.Key, c => c.ToList());

        return this.resultsByItemReceived.GetValueOrDefault(itemId);
    }

    public List<HWDGathererInspection.HWDGathererInspectionDataStruct>? GetResultsByGatheringItemId(uint gatheringItemId)
    {
        this.resultsByGatheringItem ??= this.Base.HWDGathererInspectionData.Where(c => c.RequiredItem.RowId != 0)
            .GroupBy(c => c.RequiredItem.RowId).ToDictionary(c => c.Key, c => c.ToList());

        return this.resultsByGatheringItem.GetValueOrDefault(gatheringItemId);
    }
}