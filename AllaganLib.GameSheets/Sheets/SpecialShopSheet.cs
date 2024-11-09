using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
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

public class SpecialShopRow : ExtendedRow<SpecialShop, SpecialShopRow, SpecialShopSheet>, IShop
{
    private IEnumerable<IShopListing>? shopListings;
    private IEnumerable<SpecialShopListing>? specialShopListings;
    private IEnumerable<ItemRow>? rewards;
    private IEnumerable<ItemRow>? costs;
    private List<ENpcBaseRow>? eNpcs;
    private FateShopRow? fateShopRow;
    private string? name;
    private HashSet<uint>? mapIds;

    public string Name => this.ToString();

    public IEnumerable<IShopListing> ShopListings => this.shopListings ??= this.BuildShopItems().ShopListings;

    public IEnumerable<IShopListing> SpecialShopListings => this.specialShopListings ??= this.BuildShopItems().ShopListings;

    public HashSet<uint> MapIds => this.mapIds ??= this.ENpcs.SelectMany(c => c.Locations.Select(d => d.Map.RowId)).Distinct().ToHashSet();

    public IEnumerable<ENpcBaseRow> ENpcs =>
        this.eNpcs ??= this.Sheet.GetShopIds(this.RowId)
            .Select(c => this.Sheet.GetENpcBaseSheet().GetRow(c)).ToList();

    public IEnumerable<ItemRow> Items => this.rewards ??= this.BuildShopItems().Rewards;

    public IEnumerable<ItemRow> CostItems => this.costs ??= this.BuildShopItems().Costs;

    public FateShopRow? FateShop => this.GetFateShopAdjustedRowId() == null ? null : this.fateShopRow ??= this.Sheet.GetFateShopSheet().GetRow(this.GetFateShopAdjustedRowId()!.Value);

    public bool IsFateShop => this.GetFateShopAdjustedRowId() != null;

    private uint? GetFateShopAdjustedRowId()
    {
        var adjustedId = this.Sheet.GetSpecialShopToFateShop(this.RowId);
        return adjustedId;
    }

    public override string ToString()
    {
        if (this.name == null)
        {
            var adjustedRowId = this.GetFateShopAdjustedRowId();
            if (adjustedRowId != null)
            {
                var resident = this.Sheet.GameData.GetExcelSheet<ENpcResident>()!.GetRowOrDefault(adjustedRowId.Value);
                if (resident != null)
                {
                    this.name = resident.Value.Singular.ExtractText();
                }
            }

            if (this.name == null)
            {
                var shopName = this.Sheet.GetShopName(this.GetFateShopAdjustedRowId() ?? this.RowId);
                this.name = shopName ?? this.Base.Name.ExtractText();
            }
        }

        if (this.name == string.Empty)
        {
            this.name = "Unknown Vendor";
        }

        return this.name;
    }

    private (SpecialShopListing[] ShopListings, List<ItemRow> Rewards, List<ItemRow> Costs) BuildShopItems()
    {
        var shopListingsLookup = new List<SpecialShopListing>();
        var rewardItemsLookup = new List<ItemRow>();
        var costItemsLookup = new List<ItemRow>();
        foreach (var item in this.Base.Item)
        {
            var specialShopListing = new SpecialShopListing(this, this.Sheet.GetItemSheet(), this.Sheet.TomestoneLookup, item);
            if (specialShopListing.Rewards.Any())
            {
                shopListingsLookup.Add(specialShopListing);
                rewardItemsLookup.AddRange(specialShopListing.Rewards.Select(listing => listing.Item));
                costItemsLookup.AddRange(specialShopListing.Costs.Select(listing => listing.Item));
            }
        }

        return (shopListingsLookup.ToArray(), rewardItemsLookup, costItemsLookup);
    }
}

public class SpecialShopListing : IShopListing
{
    private static Dictionary<uint, uint> currencies = new Dictionary<uint, uint>()
    {
        { 1, 10309 },
        { 2, 33913 }, // Unlimited Crafters{  scrip
        { 3, 10311 },
        { 4, 33914 }, // Unlimited Gatherers{  scrip
        { 5, 10307 },
        { 6, 41784 }, // Limited Crafters{  scrip
        { 7, 41785 }, // Limited Gatherers{  scrip
        { 8, 21072 },
        { 9, 21073 },
        { 10, 21074 },
        { 11, 21075 },
        { 12, 21076 },
        { 13, 21077 },
        { 14, 21078 },
        { 15, 21079 },
        { 16, 21080 },
        { 17, 21081 },
        { 18, 21172 },
        { 19, 21173 },
        { 20, 21935 },
        { 21, 22525 },
        { 22, 26533 },
        { 23, 26807 },
        { 24, 28063 },
        { 25, 28186 },
        { 26, 28187 },
        { 27, 28188 },
        { 28, 30341 }
    };

    private static Dictionary<uint, uint> tomeStones = new Dictionary<uint, uint>() {
        { 1, 28 },
        { 2, 46 },
        { 3, 47 },
    };

    //No fucking idea why these 2 are special, make a PR if you know how square managed to make this system even stupider
    private static HashSet<uint> _currencyShops = new HashSet<uint>()
    {
        1770637,
        1770638,
        1770699
    };

    public SpecialShopListing(
        SpecialShopRow specialShopRow,
        ItemSheet itemSheet,
        Dictionary<uint, uint> tomeStoneLookup,
        SpecialShop.ItemStruct itemDataStruct)
    {
        if (specialShopRow.RowId == 1770095)
        {
            var a = "";
        }
        var costListings = new List<ShopListingItem>();
        foreach (var costItem in itemDataStruct.ItemCosts)
        {
            if (costItem.ItemCost.RowId != 0)
            {
                var costItemId = costItem.ItemCost.RowId;
                costItemId = ConvertCurrencyId(specialShopRow.RowId, costItemId, specialShopRow.Base.UseCurrencyType);
                costListings.Add(
                    new ShopListingItem(
                        itemSheet,
                        this,
                        costItemId,
                        costItem.CurrencyCost,
                        costItem.HqCost == 1,
                        costItem.CollectabilityCost));
            }
        }

        var rewardListings = new List<ShopListingItem>();
        foreach (var receiveItem in itemDataStruct.ReceiveItems)
        {
            if (receiveItem.Item.RowId != 0)
            {
                var rewardItemId = receiveItem.Item.RowId;
                rewardListings.Add(
                    new ShopListingItem(
                        itemSheet,
                        this,
                        rewardItemId,
                        receiveItem.ReceiveCount,
                        receiveItem.ReceiveHq));
            }
        }

        this.Rewards = rewardListings;
        this.Costs = costListings;

        // uint ConvertCurrencyId(uint specialShopId, uint itemId, ushort useCurrencyType)
        // {
        //     if (itemId < 8 && itemId != 0)
        //     {
        //         switch (useCurrencyType)
        //         {
        //             case 16:
        //                 return currencies[itemId];
        //             case 8:
        //                 return 1;
        //             case 4:
        //                 return tomeStoneLookup.GetValueOrDefault(itemId, itemId);
        //             case 2:
        //                 return currencies[itemId];
        //         }
        //     }
        //
        //     return itemId;
        // }

        uint ConvertCurrencyId(uint specialShopId, uint itemId, ushort useCurrencyType)
        {
            if (specialShopId == 1770637)
            {
                if (currencies.TryGetValue(itemId, out var currencyValue))
                {
                    return currencyValue;
                }
                return itemId;
            }

            if (specialShopId == 1770446 || (specialShopId == 1770699 && itemId < 10))
            {
                if (currencies.TryGetValue(itemId, out var currencyValue) || tomeStones.TryGetValue(itemId, out currencyValue))
                {
                    return currencyValue;
                }
                return itemId;
            }

            if (useCurrencyType == 2 && itemId < 10)
            {
                if (tomeStones.TryGetValue(itemId, out var tomestoneValue))
                {
                    return tomestoneValue;
                }
                return itemId;
            }

            if ((useCurrencyType == 16 || useCurrencyType == 4) && itemId < 10)
            {
                if (tomeStones.TryGetValue(itemId, out var currencyValue) || currencies.TryGetValue(itemId, out currencyValue))
                {
                    return currencyValue;
                }
                return itemId;
            }

            return itemId;
        }
    }

    public IEnumerable<IShopListingItem> Rewards { get; set; }

    public IEnumerable<IShopListingItem> Costs { get; set; }
}