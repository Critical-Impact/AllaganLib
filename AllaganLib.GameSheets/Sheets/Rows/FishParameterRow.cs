using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class FishParameterRow : ExtendedRow<FishParameter, FishParameterRow, FishParameterSheet>
{
    private List<FishingSpotRow>? fishingSpots;

    public string FishRecordType => this.Base.FishingRecordType.Value.Addon.Value.Text.ExtractText();

    public List<FishingSpotRow> FishingSpots =>
        fishingSpots ??= this.Sheet.FishingSpotSheet.GetFishingSpots(this.Base.Item.RowId);
}