using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SpearfishingNotebookSheet : ExtendedSheet<SpearfishingNotebook, SpearfishingNotebookRow, SpearfishingNotebookSheet>, IExtendedSheet
{
    private TerritoryTypeSheet? territoryTypeSheet;
    private GatheringPointBaseSheet? gatheringPointBaseSheet;
    private Dictionary<uint,SpearfishingNotebookRow> spearfishingNotebookByGatheringPointBaseId;

    public SpearfishingNotebookSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public Dictionary<uint, SpearfishingNotebookRow> SpearfishingNotebookByGatheringPointBaseId => this.spearfishingNotebookByGatheringPointBaseId;

    public TerritoryTypeSheet GetTerritoryTypeSheet()
    {
        return this.territoryTypeSheet ??= this.SheetManager.GetSheet<TerritoryTypeSheet>();
    }

    public GatheringPointBaseSheet GetGatheringPointBaseSheet()
    {
        return this.gatheringPointBaseSheet ??= this.SheetManager.GetSheet<GatheringPointBaseSheet>();
    }

    public SpearfishingNotebookRow? GetSpearfishingNotebookByGatheringPointBaseId(uint gatheringPointBaseId)
    {
        return this.SpearfishingNotebookByGatheringPointBaseId.GetValueOrDefault(gatheringPointBaseId);
    }

    public override void CalculateLookups()
    {
        this.spearfishingNotebookByGatheringPointBaseId = this.SheetIndexer.OneToOne(
            this,
            this.SheetManager.GetSheet<GatheringPointBaseSheet>(),
            row => (row.GatheringPointBase.RowId, row));
    }
}