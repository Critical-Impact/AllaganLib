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

public class GCShopSheet : ExtendedSheet<GCShop, GCShopRow, GCShopSheet>, IExtendedSheet
{
    private readonly List<ShopName> shopNames;
    private readonly NpcShopCache shopCache;
    private readonly Dictionary<uint, ShopName> shopNamesByShopId;
    private ENpcBaseSheet? eNpcBaseSheet;
    private GCScripShopItemSheet? gcScripShopItemSheet;
    private GCScripShopCategorySheet? gcScripShopCategorySheet;

    public GCShopSheet(
        GameData gameData,
        SheetManager sheetManager,
        SheetIndexer sheetIndexer,
        ItemInfoCache itemInfoCache,
        List<ShopName> shopNames,
        NpcShopCache shopCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.shopNames = shopNames;
        this.shopCache = shopCache;
        this.shopNamesByShopId = shopNames.ToDictionary(c => c.ShopId, c => c);
    }

    public string? GetShopName(uint shopId)
    {
        return this.shopNamesByShopId.TryGetValue(shopId, out var value) ? value.Name : null;
    }

    public ENpcBaseSheet GetENpcBaseSheet()
    {
        return this.eNpcBaseSheet ??= this.SheetManager.GetSheet<ENpcBaseSheet>();
    }

    public GCScripShopItemSheet GetGCScripShopItemSheet()
    {
        return this.gcScripShopItemSheet ??= this.SheetManager.GetSheet<GCScripShopItemSheet>();
    }

    public GCScripShopCategorySheet GetGCScripShopCategorySheet()
    {
        return this.gcScripShopCategorySheet ??= this.SheetManager.GetSheet<GCScripShopCategorySheet>();
    }

    public List<uint> GetShopIds(uint shopId)
    {
        return this.shopCache.GetNpcsByGcShopId(shopId)?.ToList() ?? [];
    }

    public override void CalculateLookups()
    {
    }
}