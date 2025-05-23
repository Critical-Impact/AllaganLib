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
    private Dictionary<uint, List<(uint, uint)>> specialShopToInclusionShopSeriesLookup;
    private ENpcResidentSheet? eNpcResidentSheet;
    private ItemSheet? itemSheet;
    private FateShopSheet? fateShopSheet;
    private ENpcBaseSheet? eNpcBaseSheet;
    private InclusionShopSeriesSheet? inclusionShopSeriesSheet;
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
        this.specialShopToInclusionShopSeriesLookup = new Dictionary<uint, List<(uint, uint)>>();
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

            var tomeStones = this.GameData.GetExcelSheet<TomestonesItem>()!
                .Where(t => t.Tomestones.RowId > 0 && t.Unknown0 > 0) // Unknown0 is the InventorySlot the Currency is stored in. When it's -1 it appears to reference a field directly in InventoryManager
                .OrderBy(t => t.Tomestones.RowId)
                .ToDictionary(c => c.Tomestones.RowId, c => c.Item.RowId);

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

    public List<uint> GetInclusionShopIds(uint inclusionShopId)
    {
        return this.shopCache.GetNpcsByInclusionShopId(inclusionShopId)?.ToList() ?? [];
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public FateShopSheet GetFateShopSheet()
    {
        return this.fateShopSheet ??= this.SheetManager.GetSheet<FateShopSheet>();
    }

    public InclusionShopSeriesSheet GetInclusionShopSeriesSheet()
    {
        return this.inclusionShopSeriesSheet ??= this.SheetManager.GetSheet<InclusionShopSeriesSheet>();
    }

    public override void CalculateLookups()
    {
        this.specialShopToFateShopLookup.Clear();

        this.specialShopToFateShopLookup = this.GameData.GetExcelSheet<FateShop>()!.ToSingleLookup(
            c => c.SpecialShop.Select(d => d.RowId),
            c => c.RowId);

        this.specialShopToInclusionShopSeriesLookup.Clear();

        foreach (var row in this.GameData.GetSubrowExcelSheet<InclusionShopSeries>()!)
        {
            for (var index = 0; index < row.Count; index++)
            {
                var subRow = row[index];
                if (subRow.SpecialShop.RowId == 0)
                {
                    continue;
                }

                this.specialShopToInclusionShopSeriesLookup.TryAdd(subRow.SpecialShop.RowId, new List<(uint, uint)>());
                this.specialShopToInclusionShopSeriesLookup[subRow.SpecialShop.RowId].Add((row.RowId, (uint)index));
            }
        }
    }

    public uint? GetSpecialShopToFateShop(uint specialShopId)
    {
        if (this.specialShopToFateShopLookup.TryGetValue(specialShopId, out var value))
        {
            return value;
        }

        return null;
    }

    public List<(uint, uint)>? GetSpecialShopToInclusionShopSeries(uint specialShopId)
    {
        return this.specialShopToInclusionShopSeriesLookup.GetValueOrDefault(specialShopId);
    }
}