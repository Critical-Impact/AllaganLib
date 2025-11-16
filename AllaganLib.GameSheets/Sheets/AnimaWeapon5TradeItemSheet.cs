using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets;

public class AnimaWeapon5TradeItemSheet : ExtendedSheet<AnimaWeapon5TradeItem, AnimaWeapon5TradeItemRow, AnimaWeapon5TradeItemSheet>, IExtendedSheet
{
    private readonly NpcShopCache shopCache;
    private ENpcBaseSheet? eNpcBaseSheet;
    private ItemSheet? itemSheet;

    public AnimaWeapon5TradeItemSheet(
        GameData gameData,
        SheetManager sheetManager,
        SheetIndexer sheetIndexer,
        ItemInfoCache itemInfoCache,
        NpcShopCache shopCache)
        : base(
            gameData,
            sheetManager,
            sheetIndexer,
            itemInfoCache)
    {
        this.shopCache = shopCache;
    }

    public List<uint> GetShopIds(uint shopId)
    {
        return this.shopCache.GetAnimaShopsByNpcId(shopId)?.ToList() ?? [];
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