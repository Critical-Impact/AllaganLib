using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GCScripShopItemSheet : ExtendedSubrowSheet<GCScripShopItem, GCScripShopItemRow, GCScripShopItemSheet>, IExtendedSheet
{
    private ItemSheet? itemSheet;
    private Dictionary<uint, List<GCScripShopItemRow>>? itemRows;
    private GCScripShopCategorySheet? gcScripShopCategorySheet;

    public GCScripShopItemSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer)
    {
    }

    public List<GCScripShopItemRow>? GetItemsByGCShopId(uint gcShopId)
    {
        this.itemRows ??= this.Where(c => c.RowId != 0 && c.Item.RowId != 0).GroupBy(c => c.RowId).ToDictionary(c => c.Key, c => c.ToList());
        return this.itemRows.GetValueOrDefault(gcShopId);
    }

    public GCScripShopCategorySheet GetGCScripShopCategorySheet()
    {
        return this.gcScripShopCategorySheet ??= this.SheetManager.GetSheet<GCScripShopCategorySheet>();
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public override void CalculateLookups()
    {
    }
}