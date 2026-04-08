using AllaganLib.GameSheets.Extensions;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Model;

/// <inheritdoc />
public class TerritoryLocation : ILocation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerritoryLocation"/> class.
    /// </summary>
    /// <param name="territoryType">The territory type to create the location from.</param>
    public TerritoryLocation(TerritoryType territoryType)
    {
        this.Map = territoryType.Map;
        this.PlaceName = territoryType.PlaceName;
        this.TerritoryType = territoryType.AsTypedRowRef();
    }

    /// <inheritdoc/>
    public double MapX => 0;

    /// <inheritdoc/>
    public double MapY => 0;

    /// <inheritdoc/>
    public bool HasCoordinates => false;

    /// <inheritdoc/>
    public RowRef<Map> Map { get; }

    /// <inheritdoc/>
    public RowRef<PlaceName> PlaceName { get; }

    /// <inheritdoc/>
    public RowRef<TerritoryType> TerritoryType { get; }
}