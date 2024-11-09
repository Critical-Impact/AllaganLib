using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GatheringTypeRow : ExtendedRow<GatheringType, GatheringTypeRow, GatheringTypeSheet>
{
    private List<GatheringPointBaseRow>? gatheringPointBases;

    public List<GatheringPointBaseRow> GatheringPointBases => this.gatheringPointBases ??= this.Sheet.GetGatheringPointBaseSheet().Where(c => c.Base.GatheringType.RowId == this.RowId).ToList();
}