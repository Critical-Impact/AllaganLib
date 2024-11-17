using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets;

public class RetainerTaskSheet : ExtendedSheet<RetainerTask, RetainerTaskRow, RetainerTaskSheet>, IExtendedSheet
{
    private readonly List<RetainerVentureItem> retainerVentureItems;
    private ClassJobCategorySheet? classJobCategorySheet;
    private RetainerTaskNormalSheet? retainerTaskNormalSheet;
    private RetainerTaskRandomSheet? retainerTaskRandomSheet;
    private Dictionary<uint, List<RetainerVentureItem>> retainerVentureItemsLookup;
    private ItemSheet? itemSheet;

    public RetainerTaskSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache, List<RetainerVentureItem> retainerVentureItems)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.retainerVentureItems = retainerVentureItems;
        this.retainerVentureItemsLookup = new Dictionary<uint, List<RetainerVentureItem>>();
    }

    public override void CalculateLookups()
    {
        this.retainerVentureItemsLookup.Clear();

        this.retainerVentureItemsLookup = this.retainerVentureItems.GroupBy(c => c.RetainerTaskRandomId).ToDictionary(c => c.Key, c => c.ToList());
    }

    public List<RetainerVentureItem> GetRetainerVentureItemsByTaskRandomId(uint taskRandomId)
    {
        return this.retainerVentureItemsLookup.TryGetValue(taskRandomId, out var spawnPositions) ? spawnPositions : [];
    }

    public ClassJobCategorySheet GetClassJobCategorySheet()
    {
        return this.classJobCategorySheet ??= this.SheetManager.GetSheet<ClassJobCategorySheet>();
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public RetainerTaskNormalSheet GetRetainerTaskNormalSheet()
    {
        return this.retainerTaskNormalSheet ??= this.SheetManager.GetSheet<RetainerTaskNormalSheet>();
    }
    public RetainerTaskRandomSheet GetRetainerTaskRandomSheet()
    {
        return this.retainerTaskRandomSheet ??= this.SheetManager.GetSheet<RetainerTaskRandomSheet>();
    }
}