using System;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Model;

public class NpcLocation : ILocation, IEquatable<NpcLocation>
{
    public RowRef<Map> Map { get; }

    public RowRef<PlaceName> PlaceName { get; }

    public RowRef<TerritoryType> TerritoryType { get; }

    private readonly double X;
    private readonly double Y;
    private readonly bool AlreadyConverted;

    public NpcLocation(
        double mapX,
        double mapY,
        RowRef<Map> mapEx,
        RowRef<PlaceName> placeNameEx,
        RowRef<TerritoryType> territoryTypeEx,
        bool alreadyConverted = false)
    {
        this.X = mapX;
        this.Y = mapY;
        this.Map = mapEx;
        this.PlaceName = placeNameEx;
        this.TerritoryType = territoryTypeEx;
        this.AlreadyConverted = alreadyConverted;
    }

    /// <summary>
    ///     Gets the X-coordinate on the 2D-map.
    /// </summary>
    /// <value>The X-coordinate on the 2D-map.</value>
    public double MapX
    {
        get
        {
            if (this.AlreadyConverted)
            {
                return this.X;
            }

            if (this.Map.ValueNullable != null)
            {
                return 0;

                // return MapUtil.ConvertWorldCoordXZToMapCoord(
                //     (float)this.X,
                //     this.Map.Value.SizeFactor,
                //     this.Map.Value.OffsetX); //TODO: UNCOMMENT ME
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
            if (this.AlreadyConverted)
            {
                return this.Y;
            }

            if (this.Map.ValueNullable != null)
            {
                return 0;
                //TODO: UNCOMMENT ME
                // return MapUtil.ConvertWorldCoordXZToMapCoord(
                //     (float)this.Y,
                //     this.Map.Value.SizeFactor,
                //     this.Map.Value.OffsetY);
            }

            return 0;
        }
    }

    public string FormattedName
    {
        get
        {
            var map = this.Map.ValueNullable?.PlaceName.ValueNullable?.Name.ToString() ?? "Unknown Map";
            var region = this.Map.ValueNullable?.PlaceNameRegion.ValueNullable?.Name.ToString() ?? "Unknown Territory";
            var subArea = this.Map.ValueNullable?.PlaceNameSub.ValueNullable?.Name.ToString() ?? null;
            if (!string.IsNullOrEmpty(subArea))
            {
                subArea = " - " + subArea;
            }

            return region + " - " + map + (subArea ?? "");
        }
    }

    public override string ToString()
    {
        return this.FormattedName;
    }

    public bool Equals(NpcLocation? other)
    {
        if (other == null)
        {
            return false;
        }

        return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Map.RowId.Equals(other.Map.RowId) &&
               this.PlaceName.RowId.Equals(other.PlaceName.RowId);
    }

    public override bool Equals(object? obj)
    {
        return obj is NpcLocation other && this.Equals(other);
    }

    public bool EqualRounded(NpcLocation other)
    {
        if (this.Map.RowId.Equals(other.Map.RowId) && this.PlaceName.RowId.Equals(other.PlaceName.RowId))
        {
            var x = (int)this.MapX;
            var y = (int)this.MapY;
            var otherX = (int)other.MapX;
            var otherY = (int)other.MapY;
            if (x == otherX && y == otherY)
            {
                return true;
            }
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.X, this.Y, this.Map.RowId, this.PlaceName.RowId);
    }
}