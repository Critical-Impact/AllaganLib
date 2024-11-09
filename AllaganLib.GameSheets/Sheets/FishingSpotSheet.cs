using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class FishingSpotSheet : ExtendedSheet<FishingSpot, FishingSpotRow, FishingSpotSheet>, IExtendedSheet
{
    private ItemSheet? itemSheet;
    public FishingSpotSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public override void CalculateLookups()
    {
    }
}

public class FishingSpotRow : ExtendedRow<FishingSpot, FishingSpotRow, FishingSpotSheet>
{
    private List<ItemRow>? itemRows;

    public List<ItemRow> Items
    {
        get
        {
            return this.itemRows ??= this.Base.Item.Where(c => c.IsValid).Select(c => this.Sheet.GetItemSheet().GetRow(c.RowId)).ToList();
        }
    }
}