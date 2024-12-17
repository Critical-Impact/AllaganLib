using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class SpearfishingNotebookRow : ExtendedRow<SpearfishingNotebook, SpearfishingNotebookRow, SpearfishingNotebookSheet>, ILocation
{
    private TerritoryTypeRow? territoryType;
    private GatheringPointBaseRow? gatheringPointBase;

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
                    MapUtility.MarkerToMap(this.Base.Y, this.Map.Value.SizeFactor),
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
                    this.Base.Y,
                    this.Map.Value.SizeFactor) / 100d;
            }

            return 0;
        }
    }

    public RowRef<Map> Map => this.TerritoryType.Value.Map;

    public RowRef<PlaceName> PlaceName => this.Base.PlaceName;

    public RowRef<TerritoryType> TerritoryType => this.Base.TerritoryType;

    public TerritoryTypeRow? TerritoryTypeRow
    {
        get
        {
            return this.territoryType ??= this.Base.TerritoryType.IsValid ? this.Sheet.GetTerritoryTypeSheet().GetRow(this.Base.TerritoryType.RowId) : null;
        }
    }

    public GatheringPointBaseRow GatheringPointBase => gatheringPointBase ??=
        this.Sheet.GetGatheringPointBaseSheet().GetRow(this.Base.GatheringPointBase.RowId);
}