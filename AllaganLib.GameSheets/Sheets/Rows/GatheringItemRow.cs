using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GatheringItemRow : ExtendedRow<GatheringItem, GatheringItemRow, GatheringItemSheet>
{
    private List<GatheringPointRow>? gatheringPointRows;
    private List<GatheringPointBaseRow>? gatheringPointBases;
    private List<GatheringPointTransientRow>? gatheringPointTransientRows;
    private bool itemRowPopulated;
    private ItemRow? itemRow;

    private HashSet<uint>? mapIds;
    //private SpearfishingItemRow? spearfishingItemRow;

    public bool AvailableAtTimedNode => this.GatheringPointTransients.Any(c => c.TimedNode);

    public bool AvailableAtHiddenNode => HardcodedItems.HiddenNodeItemIds.Contains(this.Base.Item.RowId);

    public bool AvailableAtEphemeralNode => this.GatheringPointTransients.Any(d => d.EphemeralNode);

    public HashSet<uint> MapIds => this.mapIds ??= this.GatheringPoints.Select(c => c.Base.TerritoryType.ValueNullable?.Map.RowId ?? 0)
        .Where(c => c != 0).Distinct().ToHashSet();

    public ItemRow? Item
    {
        get
        {
            if (this.itemRowPopulated)
            {
                return this.itemRow;
            }

            this.itemRowPopulated = true;
            if (!this.Base.Item.Is<Item>())
            {
                return null;
            }

            return this.itemRow ??= this.Sheet.SheetManager.GetSheet<ItemSheet>().GetRow(this.Base.Item.RowId);
        }
    }

    public List<GatheringPointBaseRow> GatheringPointBases
    {
        get
        {
            if (this.gatheringPointBases != null)
            {
                return this.gatheringPointBases;
            }
            this.PopulateRelated();
            return this.gatheringPointBases!;
        }
    }

    public List<GatheringPointRow> GatheringPoints
    {
        get
        {
            if (this.gatheringPointRows != null)
            {
                return this.gatheringPointRows;
            }
            this.PopulateRelated();
            return this.gatheringPointRows!;
        }
    }

    public List<GatheringPointTransientRow> GatheringPointTransients
    {
        get
        {
            if (this.gatheringPointTransientRows != null)
            {
                return this.gatheringPointTransientRows;
            }
            this.PopulateRelated();
            return this.gatheringPointTransientRows!;
        }
    }

    private void PopulateRelated()
    {
        List<GatheringPointRow> pointRows = new();
        List<GatheringPointTransientRow> transientPointRows = new();
        var pointBases = this.Sheet.GetGatheringPointBaseSheet().GetGatheringPointBasesByGatheringItemId(this.RowId);
        if (pointBases != null)
        {
            this.gatheringPointBases = pointBases.ToList();
            var points = pointBases.SelectMany(
                c => this.Sheet.GetGatheringPointSheet().GetGatheringPointsByGatheringPointBaseId(c.RowId) ?? []).ToList();

            transientPointRows = points.Select(
                c => this.Sheet.SheetManager.GetSheet<GatheringPointTransientSheet>().GetRow(c.RowId)).Where(c => c.Base.EphemeralEndTime != 65535 || c.Base.EphemeralStartTime != 65535 || c.Base.GatheringRarePopTimeTable.RowId != 0).ToList();

            pointRows = points;
        }

        this.gatheringPointRows = pointRows;
        this.gatheringPointTransientRows = transientPointRows;
    }
}