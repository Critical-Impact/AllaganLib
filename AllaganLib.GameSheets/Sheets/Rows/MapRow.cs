using System.Collections.Generic;
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
                var map = this.Base.PlaceName.ValueNullable?.Name.ExtractText() ?? null;
                var region = this.Base.PlaceNameRegion.ValueNullable?.Name.ToString() ?? null;
                var subArea = this.Base.PlaceNameSub.ValueNullable?.Name.ToString() ?? null;

                if (region == "???")
                {
                    region = null;
                }

                var parts = new List<string>();
                if (!string.IsNullOrEmpty(region))
                {
                    parts.Add(region);
                }

                if (!string.IsNullOrEmpty(map))
                {
                    parts.Add(map);
                }

                if (!string.IsNullOrEmpty(subArea))
                {
                    parts.Add(subArea);
                }

                this.formattedName = string.Join(" - ", parts);
            }

            return this.formattedName;
        }
    }
}