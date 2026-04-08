using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GatheringPointRow : ExtendedRow<GatheringPoint, GatheringPointRow, GatheringPointSheet>, ILocation
{
    private GatheringPointBaseRow? gatheringPointBaseRow;
    private GatheringPointTransientRow? gatheringPointTransientRow;
    private GatheringPointNameRow? gatheringPointNameRow;

    public GatheringPointBaseRow GatheringPointBase => this.gatheringPointBaseRow ??= this.Sheet.GetGatheringPointBaseSheet().GetRow(this.Base.GatheringPointBase.RowId);

    public GatheringPointTransientRow GatheringPointTransient => this.gatheringPointTransientRow ??= this.Sheet.GetGatheringPointTransientSheet().GetRow(this.Base.RowId);

    public GatheringPointNameRow GatheringPointNameRow => this.gatheringPointNameRow ??=
        this.Sheet.SheetManager.GetSheet<GatheringPointNameSheet>().GetRow(this.Base.Type);

    public int GatherMarkerX
    {
        get
        {
            if (this.Map.ValueNullable != null)
            {
                return MapUtility.IntegerToInternal(
                    MapUtility.NodeToMap(this.GatheringPointBase.ExportedGatheringPoint.Base.X, this.Map.Value.SizeFactor),
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
                    MapUtility.NodeToMap(this.GatheringPointBase.ExportedGatheringPoint.Base.Y, this.Map.Value.SizeFactor),
                    this.Map.Value.SizeFactor);
            }

            return 0;
        }
    }

    /// <inheritdoc/>
    public double MapX
    {
        get
        {
            if (this.Map.ValueNullable != null)
            {
                return MapUtility.ConvertWorldCoordXZToMapCoord(
                    this.GatheringPointBase.ExportedGatheringPoint.Base.X,
                    this.Map.Value.SizeFactor,
                    this.Map.Value.OffsetX);
            }

            return 0;
        }
    }

    /// <inheritdoc/>
    public double MapY
    {
        get
        {
            if (this.Map.ValueNullable != null)
            {
                return MapUtility.ConvertWorldCoordXZToMapCoord(
                    this.GatheringPointBase.ExportedGatheringPoint.Base.Y,
                    this.Map.Value.SizeFactor,
                    this.Map.Value.OffsetY);
            }

            return 0;
        }
    }

    /// <inheritdoc/>
    public bool HasCoordinates => true;

    /// <inheritdoc/>
    public RowRef<Map> Map => this.TerritoryType.Value.Map;

    /// <inheritdoc/>
    public RowRef<PlaceName> PlaceName => this.Base.PlaceName;

    /// <inheritdoc/>
    public RowRef<TerritoryType> TerritoryType => this.Base.TerritoryType;
}