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

public class GilShopSheet : ExtendedSheet<GilShop, GilShopRow, GilShopSheet>, IExtendedSheet
{
    private readonly List<ShopName> shopNames;
    private readonly NpcShopCache shopCache;
    private readonly Dictionary<uint, ShopName> shopNamesByShopId;
    private GilShopItemSheet? gilShopItemSheet;
    private ENpcBaseSheet? eNpcBaseSheet;

    public GilShopSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache, List<ShopName> shopNames, NpcShopCache shopCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.shopNames = shopNames;
        this.shopCache = shopCache;
        this.shopNamesByShopId = shopNames.ToDictionary(c => c.ShopId, c => c);
    }

    public string? GetShopName(uint shopId)
    {
        return this.shopNamesByShopId.ContainsKey(shopId) ? this.shopNamesByShopId[shopId].Name : null;
    }

    public List<uint> GetShopIds(uint shopId)
    {
        return this.shopCache.GetNpcsByGilShopId(shopId)?.ToList() ?? [];
    }

    public GilShopItemSheet GetGilItemSheet()
    {
        return this.gilShopItemSheet ??= this.SheetManager.GetSubrowSheet<GilShopItem, GilShopItemRow, GilShopItemSheet>();
    }

    public ENpcBaseSheet GetENpcBaseSheet()
    {
        return this.eNpcBaseSheet ??= this.SheetManager.GetSheet<ENpcBaseSheet>();
    }

    public List<GilShopItemRow>? GetGilShopItems(uint shopId)
    {
        return this.GetGilItemSheet().GetItemsByShopId(shopId);
    }

    public override void CalculateLookups()
    {

    }
}