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

public class FccShopSheet : ExtendedSheet<FccShop, FccShopRow, FccShopSheet>, IExtendedSheet
{
    private readonly List<ShopName> shopNames;
    private readonly NpcShopCache shopCache;
    private readonly Dictionary<uint, ShopName> shopNamesByShopId;
    private ENpcBaseSheet? eNpcBaseSheet;
    private ItemSheet? itemSheet;

    public FccShopSheet(
        GameData gameData,
        SheetManager sheetManager,
        SheetIndexer sheetIndexer,
        ItemInfoCache itemInfoCache,
        List<ShopName> shopNames,
        NpcShopCache shopCache)
        : base(
            gameData,
            sheetManager,
            sheetIndexer,
            itemInfoCache)
    {
        this.shopNames = shopNames;
        this.shopCache = shopCache;
        this.shopNamesByShopId = shopNames.ToDictionary(c => c.ShopId, c => c);
    }

    public string? GetShopName(uint shopId)
    {
        return this.shopNamesByShopId.TryGetValue(shopId, out var value) ? value.Name : null;
    }

    public List<uint> GetShopIds(uint shopId)
    {
        return this.shopCache.GetNpcsByFccShopId(shopId)?.ToList() ?? [];
    }

    public ENpcBaseSheet GetENpcBaseSheet()
    {
        return this.eNpcBaseSheet ??= this.SheetManager.GetSheet<ENpcBaseSheet>();
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public override void CalculateLookups()
    {
    }
}