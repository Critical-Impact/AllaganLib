using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GilShopItemSheet : ExtendedSubrowSheet<GilShopItem, GilShopItemRow, GilShopItemSheet>, IExtendedSheet
{
    private Dictionary<uint, List<GilShopItemRow>>? itemsByShopId;
    private ItemSheet? itemSheet;

    public GilShopItemSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer)
    {
    }

    public List<GilShopItemRow>? GetItemsByShopId(uint shopId)
    {
        this.itemsByShopId ??= this.GroupBy(c => c.RowId).ToDictionary(c => c.Key, c => c.ToList());
        return this.itemsByShopId.GetValueOrDefault(shopId);
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public override void CalculateLookups()
    {
    }
}