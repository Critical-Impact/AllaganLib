using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GatheringItemSheet : ExtendedSheet<GatheringItem, GatheringItemRow, GatheringItemSheet>, IExtendedSheet
{
    private GatheringPointSheet? gatheringPointSheet;
    private GatheringPointBaseSheet? gatheringPointBaseSheet;
    private Dictionary<uint, List<uint>>? gatheringItemsByItemId;

    public GatheringItemSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public GatheringPointSheet GetGatheringPointSheet()
    {
        return this.gatheringPointSheet ??=
            this.SheetManager.GetSheet<GatheringPointSheet>();
    }

    public GatheringPointBaseSheet GetGatheringPointBaseSheet()
    {
        return this.gatheringPointBaseSheet ??=
            this.SheetManager.GetSheet<GatheringPointBaseSheet>();
    }

    public Dictionary<uint, List<uint>> GatheringItemsByItemId => this.gatheringItemsByItemId ??=
        this.SheetIndexer.OneToManyById(this, c => (c.Base.Item.RowId, c.RowId));

    public override void CalculateLookups()
    {
    }
}