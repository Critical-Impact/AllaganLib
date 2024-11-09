using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Time;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GatheringPointTransientSheet : ExtendedSheet<GatheringPointTransient, GatheringPointTransientRow, GatheringPointTransientSheet>, IExtendedSheet
{
    private GatheringPointBaseSheet? gatheringPointBaseSheet;
    private GatheringPointSheet? gatheringPointSheet;

    public GatheringPointTransientSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {

    }

    public GatheringPointBaseSheet GetGatheringPointBaseSheet()
    {
        return this.gatheringPointBaseSheet ??=
            this.SheetManager.GetSheet<GatheringPointBaseSheet>();
    }

    public GatheringPointSheet GetGatheringPointSheet()
    {
        return this.gatheringPointSheet ??=
            this.SheetManager.GetSheet<GatheringPointSheet>();
    }


    public override void CalculateLookups()
    {

    }

    public HashSet<uint>? GetGatheringPointBaseIdsByGatheringPointTransientId(uint gatheringPointTransientId)
    {
        return this.GetGatheringPointSheet().GetGatheringPointBaseIdsByGatheringPointId(gatheringPointTransientId);
    }

    public List<GatheringPointBaseRow>? GetGatheringPointBasesByGatheringPointTransientId(uint gatheringPointTransientId)
    {
        return this.GetGatheringPointSheet().GetGatheringPointBasesByGatheringPointId(gatheringPointTransientId);
    }

    public HashSet<uint>? GetGatheringPointTransientIdsByGatheringPointBaseId(uint gatheringPointBaseId)
    {
        return this.GetGatheringPointSheet().GetGatheringPointIdsByGatheringPointBaseId(gatheringPointBaseId);
    }

    public List<GatheringPointTransientRow>? GetGatheringPointTransientsByGatheringPointBaseId(uint gatheringPointBaseId)
    {
        return this.GetGatheringPointTransientIdsByGatheringPointBaseId(gatheringPointBaseId)?.Select(this.GetRow).ToList();
    }
}
public class GatheringPointTransientRow : ExtendedRow<GatheringPointTransient, GatheringPointTransientRow, GatheringPointTransientSheet>
{
    private BitfieldUptime? uptime;
    private bool uptimeCalculated = false;

    public bool TimedNode => this.Base.GatheringRarePopTimeTable.RowId != 0;

    public bool EphemeralNode => this.Base.EphemeralStartTime < 65535 && (this.Base.EphemeralStartTime != 0 || this.Base.EphemeralEndTime != 0);

    public BitfieldUptime? GetGatheringUptime()
    {
        if ((!this.TimedNode && !this.EphemeralNode) || this.uptimeCalculated)
        {
            return this.uptime;
        }

        // Check for ephemeral nodes
        if (this.Base.GatheringRarePopTimeTable.RowId == 0)
        {
            var time = new BitfieldUptime(this.Base.EphemeralStartTime, this.Base.EphemeralEndTime);
            this.uptime = time;
        }
        else
        {
            this.uptime = new BitfieldUptime(this.Base.GatheringRarePopTimeTable.Value);
        }

        return this.uptime;
    }
}