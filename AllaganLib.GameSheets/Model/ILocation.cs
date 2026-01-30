using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Model;

/// <summary>
/// Interface for objects defining a location in a zone (in map-coordinates).
/// </summary>
public interface ILocation
{
    /// <summary>
    /// Gets the x-coordinate of the current object.
    /// </summary>
    /// <value>The x-coordinate of the current object.</value>
    double MapX { get; }

    /// <summary>
    /// Gets the y-coordinate of the current object.
    /// </summary>
    /// <value>The y-coordinate of the current object.</value>
    double MapY { get; }

    RowRef<Map> Map { get; }

    RowRef<PlaceName> PlaceName { get; }

    RowRef<TerritoryType> TerritoryType { get; }

    public string FormattedName
    {
        get
        {
            var map = this.Map.ValueNullable?.PlaceName.ValueNullable?.Name.ToString() ?? "Unknown Map";
            var region = this.Map.ValueNullable?.PlaceNameRegion.ValueNullable?.Name.ToString() ??
                         "Unknown Territory";
            var subArea = this.Map.ValueNullable?.PlaceNameSub.ValueNullable?.Name.ToString() ?? null;
            if (!string.IsNullOrEmpty(subArea))
            {
                subArea = " - " + subArea;
            }

            return region + " - " + map + (subArea ?? string.Empty);
        }
    }
}