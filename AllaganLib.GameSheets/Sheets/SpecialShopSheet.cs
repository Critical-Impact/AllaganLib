using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets;

public class SpecialShopSheet : ExtendedSheet<SpecialShop, SpecialShopRow, SpecialShopSheet>, IExtendedSheet
{
    private readonly List<ShopName> shopNames;
    private readonly NpcShopCache shopCache;
    private readonly Dictionary<uint, ShopName> shopNamesByShopId;
    private Dictionary<uint, uint> specialShopToFateShopLookup;
    private ENpcResidentSheet? eNpcResidentSheet;
    private ItemSheet? itemSheet;
    private FateShopSheet? fateShopSheet;
    private ENpcBaseSheet? eNpcBaseSheet;
    private Dictionary<uint, uint>? tomeStonesLookup;

    public SpecialShopSheet(
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
        this.specialShopToFateShopLookup = new Dictionary<uint, uint>();
        this.shopNamesByShopId = shopNames.ToDictionary(c => c.ShopId, c => c);
    }

    public Dictionary<uint, uint> TomestoneLookup
    {
        get
        {
            if (this.tomeStonesLookup != null)
            {
                return this.tomeStonesLookup;
            }

            var items = this.GameData.GetExcelSheet<TomestonesItem>()!
                .Where(t => t.Tomestones.RowId > 0)
                .OrderBy(t => t.Tomestones.RowId)
                .ToArray();

            var tomeStones = new Dictionary<uint, uint>();

            for (uint i = 0; i < items.Length; i++) {
                tomeStones[i + 1] = items[i].Item.RowId;
            }

            this.tomeStonesLookup = tomeStones;
            return this.tomeStonesLookup;
        }
    }

    public string? GetShopName(uint shopId)
    {
        return this.shopNamesByShopId.TryGetValue(shopId, out var value) ? value.Name : null;
    }

    public ENpcResidentSheet GetENpcResidentSheet()
    {
        return this.eNpcResidentSheet ??= this.SheetManager.GetSheet<ENpcResidentSheet>();
    }

    public ENpcBaseSheet GetENpcBaseSheet()
    {
        return this.eNpcBaseSheet ??= this.SheetManager.GetSheet<ENpcBaseSheet>();
    }

    public List<uint> GetShopIds(uint shopId)
    {
        return this.shopCache.GetNpcsBySpecialShopId(shopId)?.ToList() ?? [];
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public FateShopSheet GetFateShopSheet()
    {
        return this.fateShopSheet ??= this.SheetManager.GetSheet<FateShopSheet>();
    }

    public override void CalculateLookups()
    {
        this.specialShopToFateShopLookup.Clear();

        this.specialShopToFateShopLookup = this.GameData.GetExcelSheet<FateShop>()!.ToSingleLookup(
            c => c.SpecialShop.Select(d => d.RowId),
            c => c.RowId);
    }

    public uint? GetSpecialShopToFateShop(uint specialShopId)
    {
        if (this.specialShopToFateShopLookup.TryGetValue(specialShopId, out var value))
        {
            return value;
        }

        return null;
    }
}