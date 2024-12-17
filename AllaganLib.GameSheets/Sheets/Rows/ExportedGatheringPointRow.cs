using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ExportedGatheringPointRow : ExtendedRow<ExportedGatheringPoint, ExportedGatheringPointRow, ExportedGatheringPointSheet>
{
    public int Icon => this.Base.GatheringType.Value.IconMain;
}