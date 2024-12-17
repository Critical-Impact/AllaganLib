using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class FishingSpotRow : ExtendedRow<FishingSpot, FishingSpotRow, FishingSpotSheet>, ILocation
{
    private List<ItemRow>? itemRows;

    public List<ItemRow> Items
    {
        get
        {
            return this.itemRows ??= this.Base.Item.Where(c => c.IsValid).Select(c => this.Sheet.GetItemSheet().GetRow(c.RowId)).ToList();
        }
    }

    public int GatherMarkerX
    {
        get
        {
            if (this.Map.ValueNullable != null)
            {
                return MapUtility.IntegerToInternal(
                    MapUtility.MarkerToMap(this.Base.X, this.Map.Value.SizeFactor),
                    this.Map.Value.SizeFactor);
            }

            return 0;
        }
    }

    public int GatherMarkerY
    {
        get
        {
            if (this.Map.ValueNullable != null)
            {
                return MapUtility.IntegerToInternal(
                    MapUtility.MarkerToMap(this.Base.Z, this.Map.Value.SizeFactor),
                    this.Map.Value.SizeFactor);
            }

            return 0;
        }
    }

    public double MapX
    {
        get
        {
            if (this.Map.ValueNullable != null)
            {
                return MapUtility.MarkerToMap(
                    this.Base.X,
                    this.Map.Value.SizeFactor) / 100d;
            }

            return 0;
        }
    }

    public double MapY
    {
        get
        {
            if (this.Map.ValueNullable != null)
            {
                return MapUtility.MarkerToMap(
                    this.Base.Z,
                    this.Map.Value.SizeFactor) / 100d;
            }

            return 0;
        }
    }

    public RowRef<Map> Map => this.TerritoryType.Value.Map;

    public RowRef<PlaceName> PlaceName => this.Base.PlaceName;

    public RowRef<TerritoryType> TerritoryType => this.Base.TerritoryType;
}