using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class MapRow : ExtendedRow<Map, MapRow, MapSheet>
{
    private string? formattedName;

    public string FormattedName
    {
        get
        {
            if (this.formattedName == null)
            {
                var map = this.Base.PlaceName.ValueNullable?.Name.ExtractText() ?? "Unknown Map";
                var region = this.Base.PlaceNameRegion.ValueNullable?.Name.ToString() ?? "Unknown Territory";
                var subArea = this.Base.PlaceNameSub.ValueNullable?.Name.ToString() ?? null;
                if (!string.IsNullOrEmpty(subArea))
                {
                    subArea = " - " + subArea;
                }

                this.formattedName = region + " - " + map + (subArea ?? string.Empty);
            }

            return this.formattedName;
        }
    }
}