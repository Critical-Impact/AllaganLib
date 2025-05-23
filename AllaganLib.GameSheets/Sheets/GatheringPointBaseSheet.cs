using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Helpers;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GatheringPointBaseSheet : ExtendedSheet<GatheringPointBase, GatheringPointBaseRow, GatheringPointBaseSheet>, IExtendedSheet
{
    private Dictionary<uint, HashSet<uint>> gatheringItemIdsByGatheringPointBaseId;
    private Dictionary<uint, HashSet<uint>> gatheringPointBaseIdsByItemIds;
    private Dictionary<uint, HashSet<uint>> spearfishingItemsIdsByGatheringPointBaseId;
    private Dictionary<uint, HashSet<uint>> gatheringPointBaseIdsBySpearfishingItemIds;
    private GatheringItemSheet? gatheringItemSheet;
    private SpearfishingItemSheet? spearfishingItemSheet;
    private SpearfishingNotebookSheet? spearfishingNotebookSheet;
    private SubrowExcelSheet<GatheringItemPoint>? gatheringItemPointSheet;

    public GatheringPointBaseSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.gatheringItemIdsByGatheringPointBaseId = new();
        this.gatheringPointBaseIdsByItemIds = new();
        this.spearfishingItemsIdsByGatheringPointBaseId = new();
        this.gatheringPointBaseIdsBySpearfishingItemIds = new();
    }

    public GatheringItemSheet GetGatheringItemSheet()
    {
        return this.gatheringItemSheet ??= this.SheetManager.GetSheet<GatheringItemSheet>();
    }

    public SubrowExcelSheet<GatheringItemPoint> GetGatheringItemPointSheet()
    {
        return this.gatheringItemPointSheet ??= this.GameData.GetSubrowExcelSheet<GatheringItemPoint>()!;
    }

    public SpearfishingItemSheet GetSpearfishingItemSheet()
    {
        return this.spearfishingItemSheet ??= this.SheetManager.GetSheet<SpearfishingItemSheet>();
    }

    public SpearfishingNotebookSheet GetSpearfishingNotebookSheet()
    {
        return this.spearfishingNotebookSheet ??= this.SheetManager.GetSheet<SpearfishingNotebookSheet>();
    }

    public override void CalculateLookups()
    {
        foreach (var gatheringPointBase in this)
        {
            foreach (var item in gatheringPointBase.Base.Item)
            {
                if (item.RowId == 0)
                {
                    continue;
                }

                if (item.Is<GatheringItem>())
                {
                    this.gatheringItemIdsByGatheringPointBaseId.TryAdd(gatheringPointBase.RowId, []);
                    this.gatheringItemIdsByGatheringPointBaseId[gatheringPointBase.RowId].Add(item.RowId);

                    this.gatheringPointBaseIdsByItemIds.TryAdd(item.RowId, []);
                    this.gatheringPointBaseIdsByItemIds[item.RowId].Add(gatheringPointBase.RowId);
                }
                else if (item.Is<SpearfishingItem>())
                {
                    this.spearfishingItemsIdsByGatheringPointBaseId.TryAdd(gatheringPointBase.RowId, []);
                    this.spearfishingItemsIdsByGatheringPointBaseId[gatheringPointBase.RowId].Add(item.RowId);

                    this.gatheringPointBaseIdsBySpearfishingItemIds.TryAdd(item.RowId, []);
                    this.gatheringPointBaseIdsBySpearfishingItemIds[item.RowId].Add(gatheringPointBase.RowId);
                }
            }
        }

        foreach (var gatheringItemPoint in this.GetGatheringItemPointSheet())
        {
            foreach (var subRow in gatheringItemPoint)
            {
                if (subRow.GatheringPoint.RowId != 0)
                {
                    var gatheringPointBase = subRow.GatheringPoint.Value.GatheringPointBase;
                    if (gatheringPointBase.RowId != 0)
                    {
                        this.gatheringItemIdsByGatheringPointBaseId.TryAdd(gatheringPointBase.RowId, []);
                        this.gatheringItemIdsByGatheringPointBaseId[gatheringPointBase.RowId].Add(gatheringItemPoint.RowId);

                        this.gatheringPointBaseIdsByItemIds.TryAdd(gatheringItemPoint.RowId, []);
                        this.gatheringPointBaseIdsByItemIds[gatheringItemPoint.RowId].Add(gatheringPointBase.RowId);
                    }
                }
            }
        }

        foreach (var item in HardcodedItems.GatheringPointBaseToGatheringItem)
        {
            this.gatheringItemIdsByGatheringPointBaseId.TryAdd(item.Key, []);
            this.gatheringItemIdsByGatheringPointBaseId[item.Key].Add(item.Value);

            this.gatheringPointBaseIdsByItemIds.TryAdd(item.Value, []);
            this.gatheringPointBaseIdsByItemIds[item.Value].Add(item.Key);
        }
    }

    public HashSet<uint>? GetGatheringItemIdsByGatheringPointBaseId(uint gatheringPointBaseId)
    {
        return this.gatheringItemIdsByGatheringPointBaseId.GetValueOrDefault(gatheringPointBaseId);
    }

    public List<GatheringItemRow>? GetGatheringItemsByGatheringPointBaseId(uint gatheringPointBaseId)
    {
        return this.gatheringItemIdsByGatheringPointBaseId.GetValueOrDefault(gatheringPointBaseId)?.Select(c => this.GetGatheringItemSheet().GetRow(c)).ToList();
    }

    public HashSet<uint>? GetGatheringPointBaseIdsByGatheringItemId(uint gatheringItemId)
    {
        return this.gatheringPointBaseIdsByItemIds.GetValueOrDefault(gatheringItemId);
    }

    public List<GatheringPointBaseRow>? GetGatheringPointBasesByGatheringItemId(uint gatheringItemId)
    {
        return this.gatheringPointBaseIdsByItemIds.GetValueOrDefault(gatheringItemId)?.Select(this.GetRow).ToList();
    }

    public HashSet<uint>? GetSpearfishingItemIdsByGatheringPointBaseId(uint gatheringPointBaseId)
    {
        return this.spearfishingItemsIdsByGatheringPointBaseId.GetValueOrDefault(gatheringPointBaseId);
    }

    public List<SpearfishingItemRow>? GetSpearfishingItemsByGatheringPointBaseId(uint gatheringPointBaseId)
    {
        return this.spearfishingItemsIdsByGatheringPointBaseId.GetValueOrDefault(gatheringPointBaseId)?.Select(c => this.GetSpearfishingItemSheet().GetRow(c)).ToList();
    }

    public HashSet<uint>? GetGatheringPointBaseIdsBySpearfishingItemId(uint spearfishingItemId)
    {
        return this.gatheringPointBaseIdsBySpearfishingItemIds.GetValueOrDefault(spearfishingItemId);
    }

    public List<GatheringPointBaseRow>? GetGatheringPointBasesBySpearfishingItemId(uint spearfishingItemId)
    {
        return this.gatheringPointBaseIdsBySpearfishingItemIds.GetValueOrDefault(spearfishingItemId)?.Select(this.GetRow).ToList();
    }
}