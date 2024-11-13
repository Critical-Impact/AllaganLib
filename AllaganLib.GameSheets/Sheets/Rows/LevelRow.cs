using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class LevelRow : ExtendedRow<Level, LevelRow, LevelSheet>
{
    public string FormattedName
    {
        get
        {
            var map = this.Base.Map.ValueNullable?.PlaceName.ValueNullable?.Name.ToString() ?? "Unknown Map";
            var region = this.Base.Map.ValueNullable?.PlaceNameRegion.ValueNullable?.Name.ToString() ??
                         "Unknown Territory";
            var subArea = this.Base.Map.ValueNullable?.PlaceNameSub.ValueNullable?.Name.ToString() ?? null;
            if (!string.IsNullOrEmpty(subArea))
            {
                subArea = " - " + subArea;
            }

            return region + " - " + map + (subArea ?? string.Empty);
        }
    }

    /// <summary>
    ///     Gets the X-coordinate on the 2D-map.
    /// </summary>
    /// <value>The X-coordinate on the 2D-map.</value>
    public double MapX
    {
        get
        {
            if (this.Base.Map.IsValid)
            {
                return MapUtility.ConvertWorldCoordXZToMapCoord(
                    this.Base.X,
                    this.Base.Map.Value.SizeFactor,
                    this.Base.Map.Value.OffsetX);
            }

            return 0;
        }
    }

    /// <summary>
    ///     Gets the Y-coordinate on the 2D-map.
    /// </summary>
    /// <value>The Y-coordinate on the 2D-map.</value>
    public double MapY
    {
        get
        {
            if (this.Base.Map.IsValid)
            {
                return MapUtility.ConvertWorldCoordXZToMapCoord(
                    this.Base.Z,
                    this.Base.Map.Value.SizeFactor,
                    this.Base.Map.Value.OffsetY);
            }

            return 0;
        }
    }

    public override string ToString()
    {
        return this.FormattedName;
    }
}