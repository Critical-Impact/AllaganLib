using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
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