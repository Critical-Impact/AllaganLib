using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GatheringPointBaseRow : ExtendedRow<GatheringPointBase, GatheringPointBaseRow, GatheringPointBaseSheet>
{
    public List<GatheringItemRow>? GatheringItems => this.Sheet.GetGatheringItemsByGatheringPointBaseId(this.RowId);

    public List<SpearfishingItemRow>? SpearfishingItems => this.Sheet.GetSpearfishingItemsByGatheringPointBaseId(this.RowId);

    public ExportedGatheringPointRow ExportedGatheringPoint => this.Sheet.SheetManager.GetSheet<ExportedGatheringPointSheet>().GetRow(this.RowId);

    public SpearfishingNotebookRow? SpearfishingNotebook => this.Sheet.GetSpearfishingNotebookSheet().GetSpearfishingNotebookByGatheringPointBaseId(this.RowId);
}