using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GatheringPointSheet : ExtendedSheet<GatheringPoint, GatheringPointRow, GatheringPointSheet>, IExtendedSheet
{
    private Dictionary<uint, HashSet<uint>> gatheringPointBaseIdsByGatheringPointId;
    private Dictionary<uint, HashSet<uint>> gatheringPointIdsByGatheringPointBaseId;
    private Dictionary<uint, HashSet<uint>> gatheringPointIdsByItemId;
    private Dictionary<uint, HashSet<uint>> itemIdsByGatheringPointId;
    private GatheringPointBaseSheet? gatheringPointBaseSheet;
    private GatheringPointTransientSheet? gatheringPointTransientSheet;

    public GatheringPointSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(
            gameData,
            sheetManager,
            sheetIndexer,
            itemInfoCache)
    {
        this.gatheringPointBaseIdsByGatheringPointId = [];
        this.gatheringPointIdsByGatheringPointBaseId = [];
        this.gatheringPointIdsByItemId = [];
        this.itemIdsByGatheringPointId = [];
    }

    public GatheringPointBaseSheet GetGatheringPointBaseSheet()
    {
        return this.gatheringPointBaseSheet ??=
            this.SheetManager.GetSheet<GatheringPointBaseSheet>();
    }

    public GatheringPointTransientSheet GetGatheringPointTransientSheet()
    {
        return this.gatheringPointTransientSheet ??=
            this.SheetManager.GetSheet<GatheringPointTransientSheet>();
    }

    public override void CalculateLookups()
    {
        foreach (var gatheringPoint in this)
        {
            var gatheringPointBase = gatheringPoint.Base.GatheringPointBase;
            if (gatheringPointBase.RowId == 0)
            {
                continue;
            }

            this.gatheringPointBaseIdsByGatheringPointId.TryAdd(gatheringPoint.RowId, []);
            this.gatheringPointBaseIdsByGatheringPointId[gatheringPoint.RowId].Add(gatheringPointBase.RowId);

            this.gatheringPointIdsByGatheringPointBaseId.TryAdd(gatheringPointBase.RowId, []);
            this.gatheringPointIdsByGatheringPointBaseId[gatheringPointBase.RowId].Add(gatheringPoint.RowId);
        }
    }

    public HashSet<uint>? GetGatheringPointBaseIdsByGatheringPointId(uint gatheringPointId)
    {
        return this.gatheringPointBaseIdsByGatheringPointId.GetValueOrDefault(gatheringPointId);
    }

    public List<GatheringPointBaseRow>? GetGatheringPointBasesByGatheringPointId(uint gatheringPointId)
    {
        return this.gatheringPointBaseIdsByGatheringPointId.GetValueOrDefault(gatheringPointId)
            ?.Select(c => this.GetGatheringPointBaseSheet().GetRow(c)).ToList();
    }

    public HashSet<uint>? GetGatheringPointIdsByGatheringPointBaseId(uint gatheringPointBaseId)
    {
        return this.gatheringPointIdsByGatheringPointBaseId.GetValueOrDefault(gatheringPointBaseId);
    }

    public List<GatheringPointRow>? GetGatheringPointsByGatheringPointBaseId(uint gatheringPointBaseId)
    {
        return this.gatheringPointIdsByGatheringPointBaseId.GetValueOrDefault(gatheringPointBaseId)?.Select(this.GetRow)
            .ToList();
    }
}