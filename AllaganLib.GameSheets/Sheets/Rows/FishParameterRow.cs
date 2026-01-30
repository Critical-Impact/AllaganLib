using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class FishParameterRow : ExtendedRow<FishParameter, FishParameterRow, FishParameterSheet>
{
    private List<FishingSpotRow>? fishingSpots;

    public string FishRecordType => this.Base.FishingRecordType.Value.Addon.Value.Text.ExtractText();

    public List<FishingSpotRow> FishingSpots =>
        this.fishingSpots ??= this.Sheet.FishingSpotSheet.GetFishingSpots(this.Base.Item.RowId);
}