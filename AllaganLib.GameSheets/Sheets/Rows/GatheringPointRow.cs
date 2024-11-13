using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GatheringPointRow : ExtendedRow<GatheringPoint, GatheringPointRow, GatheringPointSheet>
{
    private GatheringPointBaseRow? gatheringPointBaseRow;
    private GatheringPointTransientRow? gatheringPointTransientRow;

    public GatheringPointBaseRow GatheringPointBase => this.gatheringPointBaseRow ??= this.Sheet.GetGatheringPointBaseSheet().GetRow(this.Base.GatheringPointBase.RowId);

    public GatheringPointTransientRow GatheringPointTransient => this.gatheringPointTransientRow ??= this.Sheet.GetGatheringPointTransientSheet().GetRow(this.Base.RowId);
}