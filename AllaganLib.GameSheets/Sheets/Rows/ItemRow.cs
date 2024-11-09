using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Helpers;
using AllaganLib.GameSheets.Sheets.ItemSources;
using AllaganLib.Shared.Extensions;
using AllaganLib.Shared.Time;
using CriticalCommonLib.Enums;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public partial class ItemRow : ExtendedRow<Item, ItemRow, ItemSheet>
{
    private string? _searchString;
    private GatheringItemRow? gatheringItem;
    private EquipSlotCategoryRow? equipSlotCategory;
    private List<GatheringTypeRow>? gatheringTypes;
    private List<GatheringItemRow>? gatheringItems;
    private List<GatheringPointTransientRow>? gatheringPointTransients;
    private List<BitfieldUptime>? gatheringUpTimes;
    private List<ItemSource>? sources;
    private List<ItemSource>? uses;
    private List<SpecialShopRow>? specialShops;
    private List<FccShopRow>? fccShops;
    private List<GilShopRow>? gilShops;
    private List<GilShopRow>? calamitySalvagerShops;
    private List<GCShopRow>? gcShops;
    private List<FishingSpotRow>? fishingSpots;
    private Dictionary<ItemInfoType, HashSet<uint>>? sourceMapLocationsByType;
    private Dictionary<ItemInfoType, HashSet<uint>>? useMapLocationsByType;
    private ClassJobCategoryRow? classJobCategory;

    public uint RowId => this.Base.RowId;

    public string SearchString
    {
        get
        {
            if (this._searchString == null)
            {
                this._searchString = this.NameString.ToParseable();
            }

            return this._searchString;
        }
    }

    public EquipSlotCategoryRow? EquipSlotCategory => this.equipSlotCategory ??=
        this.Sheet.GetEquipSlotCategorySheet().GetRowOrDefault(this.Base.EquipSlotCategory.RowId);

    public ClassJobCategoryRow? ClassJobCategory => this.classJobCategory ??=
        this.Sheet.GetClassJobCategorySheet().GetRowOrDefault(this.Base.ClassJobCategory.RowId);

    //Identification

    public bool IsCrystal => this.Base.ItemUICategory.RowId == 59;

    public bool IsVenture => this.RowId == 21072;

    public bool IsGil => this.RowId == 1;

    public bool IsCompanySeal => this.RowId is 20 or 21 or 22;

    public bool IsCurrency => (this.IsVenture || this.IsCompanySeal || this.SpentSpecialShop || this.IsGil) && this.Base.ItemUICategory.RowId != 59;

    public decimal Patch => this.Sheet.GetItemPatch(this.RowId);

    public string FormattedRarity
    {
        get
        {
            switch (this.Base.Rarity)
            {
                case 1:
                    return "Normal";
                case 2:
                    return "Scarce";
                case 3:
                    return "Artifact";
                case 4:
                    return "Relic";
                case 7:
                    return "Aetherial";
                default:
                    return "Unknown";
            }
        }
    }

    //Armoire/Cabinet

    public CabinetCategoryRow? CabinetCategory => this.Sheet.GetCabinetCategory(this.RowId);

    //Acqusition
    public List<ItemSource> Sources => this.sources ??= this.Sheet.GetItemSources(this.RowId);

    public List<ItemSource> Uses => this.uses ??= this.Sheet.GetItemUses(this.RowId);

    public List<T> GetSourcesByType<T>(ItemInfoType type)
        where T : ItemSource
        => this.Sources.Where(c => c.Type == type).Cast<T>().ToList();

    public List<T> GetSourcesByType<T>(params ItemInfoType[] types)
        where T : ItemSource
        => this.Sources.Where(c => types.Contains(c.Type)).Cast<T>().ToList();

    public bool HasSourcesByType(params ItemInfoType[] types) => this.Sources.Any(c => types.Contains(c.Type));

    public bool HasUsesByType(params ItemInfoType[] types) => this.Uses.Any(c => types.Contains(c.Type));

    public bool HasUsesByCategory(ItemInfoCategory category) => this.Uses.Select(c => c.Type).InCategory(category);

    public List<T> GetSourcesByCategory<T>(ItemInfoCategory category) where T : ItemSource
    {
        var allTypes = Enum.GetValues<ItemInfoType>();
        switch (category)
        {
            case ItemInfoCategory.Gathering:
                return this.GetSourcesByType<T>(allTypes.Where(c => c.IsGathering()).ToArray());
            case ItemInfoCategory.RegularGathering:
                return this.GetSourcesByType<T>(allTypes.Where(c => c.IsRegularGathering()).ToArray());
            case ItemInfoCategory.EphemeralGathering:
                return this.GetSourcesByType<T>(allTypes.Where(c => c.IsEphemeralGathering()).ToArray());
            case ItemInfoCategory.HiddenGathering:
                return this.GetSourcesByType<T>(allTypes.Where(c => c.IsHiddenGathering()).ToArray());
            case ItemInfoCategory.TimedGathering:
                return this.GetSourcesByType<T>(allTypes.Where(c => c.IsTimedGathering()).ToArray());
            case ItemInfoCategory.NormalVenture:
                return this.GetSourcesByType<T>(allTypes.Where(c => c.IsNormalVenture()).ToArray());
            case ItemInfoCategory.ExplorationVenture:
                return this.GetSourcesByType<T>(allTypes.Where(c => c.IsExplorationVenture()).ToArray());
            case ItemInfoCategory.AllVentures:
                return this.GetSourcesByType<T>(allTypes.Where(c => c.IsExplorationVenture() || c.IsNormalVenture() || c == ItemInfoType.QuickVenture).ToArray());
            case ItemInfoCategory.Shop:
                return this.GetSourcesByType<T>(allTypes.Where(c => c.IsShop()).ToArray());
        }

        return new List<T>();
    }

    public bool HasSourcesByCategory(ItemInfoCategory category)
    {
        var allTypes = Enum.GetValues<ItemInfoType>();
        switch (category)
        {
            case ItemInfoCategory.Gathering:
                return this.HasSourcesByType(allTypes.Where(c => c.IsGathering()).ToArray());
            case ItemInfoCategory.RegularGathering:
                return this.HasSourcesByType(allTypes.Where(c => c.IsRegularGathering()).ToArray());
            case ItemInfoCategory.EphemeralGathering:
                return this.HasSourcesByType(allTypes.Where(c => c.IsEphemeralGathering()).ToArray());
            case ItemInfoCategory.HiddenGathering:
                return this.HasSourcesByType(allTypes.Where(c => c.IsHiddenGathering()).ToArray());
            case ItemInfoCategory.TimedGathering:
                return this.HasSourcesByType(allTypes.Where(c => c.IsTimedGathering()).ToArray());
            case ItemInfoCategory.NormalVenture:
                return this.HasSourcesByType(allTypes.Where(c => c.IsNormalVenture()).ToArray());
            case ItemInfoCategory.ExplorationVenture:
                return this.HasSourcesByType(allTypes.Where(c => c.IsExplorationVenture()).ToArray());
            case ItemInfoCategory.AllVentures:
                return this.HasSourcesByType(allTypes.Where(c => c.IsExplorationVenture() || c.IsNormalVenture() || c == ItemInfoType.QuickVenture).ToArray());
            case ItemInfoCategory.Shop:
                return this.HasSourcesByType(allTypes.Where(c => c.IsShop()).ToArray());
        }

        return false;
    }

    public Dictionary<ItemInfoType, HashSet<uint>> SourceMapLocationsByType =>
        this.sourceMapLocationsByType ??=
        this.Sheet.ItemInfoCache.GetItemSourceMapLocationsByItemId(this.RowId);

    public Dictionary<ItemInfoType, HashSet<uint>> UseMapLocationsByType =>
        this.useMapLocationsByType ??=
        this.Sheet.ItemInfoCache.GetItemUseMapLocationsByItemId(this.RowId);

    public bool ObtainedGathering => this.sources?.Any(c => c.Type.IsGathering()) ?? false;

    public bool ObtainedFishing => this.sources?.Any(c => c.Type == ItemInfoType.Fishing) ?? false;

    public bool ObtainedSpearFishing => this.sources?.Any(c => c.Type == ItemInfoType.Spearfishing) ?? false;

    public bool ObtainedNormalVenture => this.sources?.Any(c => c.Type.IsNormalVenture()) ?? false;

    public bool ObtainedExplorationVenture => this.sources?.Any(c => c.Type.IsExplorationVenture()) ?? false;

    //Usage

    public bool CanBeTraded => this.Base is { IsUntradable: false };

    public bool CanBeDesynthed => this.Base.Desynth != 0;

    public bool CanBePlacedOnMarket => this.Base.ItemSearchCategory.RowId != 0;

    public uint SellToVendorPrice => this.Base.PriceLow;

    public uint BuyFromVendorPrice => this.Base.PriceMid;

    public uint SellToVendorPriceHQ => this.Base.PriceLow + 1;

    public uint BuyFromVendorPriceHQ => this.Base.PriceMid + 1;

    public bool CanTryOn
    {
        get
        {
            if (!this.Base.EquipSlotCategory.IsValid)
            {
                return false;
            }

            if (this.Base.EquipSlotCategory.RowId > 0 && this.Base.EquipSlotCategory.RowId != 6 &&
                this.Base.EquipSlotCategory.RowId != 17 &&
                (this.Base.EquipSlotCategory.Value.OffHand <= 0 || this.Base.ItemUICategory.RowId == 11))
            {
                return true;
            }

            return false;
        }
    }

    public EquipRaceCategoryRow? EquipRaceCategory => this.Sheet.GetEquipRaceCategorySheet().GetRowOrDefault(this.Base.EquipRestriction);

    public CharacterRace EquipRace => this.EquipRaceCategory?.EquipRace ?? CharacterRace.None;

    public CharacterSex EquippableByGender
    {
        get
        {
            if (this.CanBeEquippedByRaceGender(CharacterRace.Any, CharacterSex.Both))
            {
                return CharacterSex.Both;
            }

            if (this.CanBeEquippedByRaceGender(CharacterRace.Any, CharacterSex.Male))
            {
                return CharacterSex.Male;
            }

            if (this.CanBeEquippedByRaceGender(CharacterRace.Any, CharacterSex.Female))
            {
                return CharacterSex.Female;
            }

            return CharacterSex.NotApplicable;
        }
    }

    public bool CanBeEquippedByRaceGender(CharacterRace race, CharacterSex sex)
    {
        if (this.Base.EquipRestriction == 0)
        {
            return false;
        }

        var equipRaceCategory = this.EquipRaceCategory;
        if (equipRaceCategory == null)
        {
            return false;
        }

        return equipRaceCategory.AllowsRaceSex(race, sex);
    }

    //Currency

    public bool SpentSpecialShop => this.Uses.Any(c => c.Type == ItemInfoType.SpecialShop);

    public bool SpentGilShop => this.Uses.Any(c => c.Type is ItemInfoType.GilShop or ItemInfoType.CalamitySalvagerShop);

    //Locations

    public HashSet<uint> GetSourceMaps(HashSet<ItemInfoType> preferenceTypes, uint? itemId = null)
    {
        var itemSources = this.Sheet.ItemInfoCache.GetItemSources(this.RowId, itemId);
        if (itemSources == null)
        {
            return new();
        }

        return itemSources.Where(c => preferenceTypes.Contains(c.Type)).Select(c => c.MapIds).Where(c => c != null).SelectMany(c => c!).ToHashSet();
    }

    public HashSet<uint> GetSourceMaps(ItemInfoType preferenceType, uint? itemId = null)
    {
        var itemSources = this.Sheet.ItemInfoCache.GetItemSources(this.RowId, itemId);
        if (itemSources == null)
        {
            return new();
        }

        return itemSources.Where(c => c.Type == preferenceType).Select(c => c.MapIds).Where(c => c != null).SelectMany(c => c!).ToHashSet();
    }

    //
    // public HashSet<uint> GetSourceMaps(ItemInfoType preferenceType, uint? itemId)
    // {
    //     if (this.sourceMaps == null)
    //     {
    //         this.sourceMaps = new();
    //     }
    //
    //     var sourceKey = (preferenceType, itemId);
    //     if (!this.sourceMaps.ContainsKey(sourceKey))
    //     {
    //         HashSet<uint> maps = new();
    //         if (preferenceType == IngredientPreferenceType.Buy)
    //         {
    //             var vendorsBySourceItemId = VendorsBySourceItemId(1);
    //             if (vendorsBySourceItemId.Count != 0)
    //             {
    //                 foreach (var map in vendorsBySourceItemId.SelectMany(c => c.ENpcs)
    //                              .SelectMany(c => c.Locations).Select(c => c.MapEx.Value).Distinct())
    //                 {
    //                     if (map == null) continue;
    //                     maps.Add(map.RowId);
    //                 }
    //             }
    //         }
    //         //TODO: Add in house vendors here somehow or maybe from wherever calls this
    //         else if (preferenceType == IngredientPreferenceType.Item && itemId != null)
    //         {
    //             var vendorsBySourceItemId = VendorsBySourceItemId(itemId.Value);
    //             if (vendorsBySourceItemId.Count != 0)
    //             {
    //                 foreach (var map in vendorsBySourceItemId.SelectMany(c => c.ENpcs)
    //                              .SelectMany(c => c.Locations).Select(c => c.MapEx.Value).Distinct())
    //                 {
    //                     if (map == null) continue;
    //                     maps.Add(map.RowId);
    //                 }
    //             }
    //         }
    //         else if (preferenceType == IngredientPreferenceType.Mobs)
    //         {
    //             var locations = MobDrops.SelectMany(c => c.MobSpawnPositions).Select(c => c.MapEx.Value).Distinct()
    //                 .ToList();
    //             if (locations.Count != 0)
    //             {
    //                 foreach (var map in locations)
    //                 {
    //                     if (map == null) continue;
    //                     maps.Add(map.RowId);
    //                 }
    //             }
    //         }
    //         else if (preferenceType == IngredientPreferenceType.Botany)
    //         {
    //             var gatheringSources = GetGatheringSources();
    //             foreach (var gatheringSource in gatheringSources)
    //             {
    //                 if (gatheringSource.IsHarvesting || gatheringSource.IsLogging)
    //                 {
    //                     maps.Add(gatheringSource.TerritoryType.Map.Row);
    //                 }
    //             }
    //         }
    //         else if (preferenceType == IngredientPreferenceType.Mining)
    //         {
    //             var gatheringSources = GetGatheringSources();
    //             foreach (var gatheringSource in gatheringSources)
    //             {
    //                 if (gatheringSource.IsMining || gatheringSource.IsQuarrying)
    //                 {
    //                     maps.Add(gatheringSource.TerritoryType.Map.Row);
    //                 }
    //             }
    //         }
    //
    //         this.sourceMaps[sourceKey] = maps;
    //     }
    //     return this.sourceMaps[sourceKey];
    // }

    //Vendors

    public List<SpecialShopRow> SpecialShops
    {
        get
        {
            if (this.specialShops == null)
            {
                this.CacheVendorLookup();
            }

            return this.specialShops!;
        }
    }

    public List<FccShopRow> FCCShops
    {
        get
        {
            if (this.fccShops == null)
            {
                this.CacheVendorLookup();
            }

            return this.fccShops!;
        }
    }

    public List<GilShopRow> GilShops
    {
        get
        {
            if (this.gilShops == null)
            {
                this.CacheVendorLookup();
            }

            return this.gilShops!;
        }
    }


    public List<GilShopRow> CalamitySalvagerShops
    {
        get
        {
            if (this.calamitySalvagerShops == null)
            {
                this.CacheVendorLookup();
            }

            return this.calamitySalvagerShops!;
        }
    }

    private void CacheVendorLookup()
    {
        this.specialShops = [];
        this.fccShops = [];
        this.gilShops = [];
        this.gcShops = [];
        this.calamitySalvagerShops = [];

        foreach (var use in this.Sources)
        {
            if (use.Type == ItemInfoType.SpecialShop)
            {
                var shopRow = (use as ItemSpecialShopSource)?.SpecialShop;
                if (shopRow != null)
                {
                    this.specialShops.Add(shopRow);
                }
            }
            else if (use.Type == ItemInfoType.GilShop)
            {
                var shopRow = (use as ItemGilShopSource)?.GilShop;
                if (shopRow != null)
                {
                    this.gilShops.Add(shopRow);
                }
            }
            else if (use.Type == ItemInfoType.CalamitySalvagerShop)
            {
                var shopRow = (use as ItemGilShopSource)?.GilShop;
                if (shopRow != null)
                {
                    this.calamitySalvagerShops.Add(shopRow);
                }
            }
            else if (use.Type == ItemInfoType.FCShop)
            {
                var shopRow = (use as ItemFccShopSource)?.FccShop;
                if (shopRow != null)
                {
                    this.fccShops.Add(shopRow);
                }
            }
            else if (use.Type == ItemInfoType.GCShop)
            {
                var shopRow = (use as ItemGCShopSource)?.GcShop;
                if (shopRow != null)
                {
                    this.gcShops.Add(shopRow);
                }
            }
        }
    }


    //CraftRecipe
    public bool CanBeCrafted => this.Sheet.SheetManager.GetSheet<RecipeSheet>().HasRecipesByItemId(this.RowId) || this.CompanyCraftSequence != null;

    public List<RecipeRow> Recipes => this.Sheet.SheetManager.GetSheet<RecipeSheet>().GetRecipesByItemId(this.RowId) ?? new List<RecipeRow>();

    public List<RecipeRow> RecipesAsRequirement => this.Sheet.SheetManager.GetSheet<RecipeSheet>().GetRecipesByIngredientItemId(this.RowId) ?? new List<RecipeRow>();

    public CompanyCraftSequenceRow? CompanyCraftSequence => this.Sheet.SheetManager.GetSheet<CompanyCraftSequenceSheet>().GetByItemId(this.RowId);

    //Gathering
    public List<GatheringItemRow> GatheringItems => this.gatheringItems ??= this.Sheet.GetGatheringItems(this.RowId) ?? new List<GatheringItemRow>();

    public List<GatheringPointTransientRow> GatheringPointTransients => this.gatheringPointTransients ??=
        this.GatheringItems.SelectMany(c => c.GatheringPointTransients.ToList()).ToList();

    public List<GatheringTypeRow> GatheringTypes => this.gatheringTypes ??= this.GenerateGatheringTypes();

    public List<BitfieldUptime> GatheringUpTimes => this.gatheringUpTimes ??= this.GatheringPointTransients.Select(c => c.GetGatheringUptime()).Where(c => c != null).Select(c => c!.Value).Distinct().ToList();

    public List<FishingSpotRow> FishingSpots => this.fishingSpots ??= this.Sheet.GetFishingSpots(this.RowId) ?? [];

    public List<SpearfishingItemRow> SpearfishingRows => this.Uses.Where(c => c.Type == ItemInfoType.Spearfishing)
        .Cast<ItemSpearfishingSource>().Select(c => c.SpearfishingItemRow).ToList();

    public bool AvailableAtTimedNode => this.GatheringItems?.Any(c => c.AvailableAtTimedNode) ?? false;

    public bool AvailableAtHiddenNode => HardcodedItems.HiddenNodeItemIds.Contains(this.RowId);

    public bool AvailableAtEphemeralNode => this.GatheringItems?.Any(c => c.AvailableAtEphemeralNode) ?? false;

    public string NameString => this.Base.Name.ExtractText();

    public string GarlandToolsId
    {
        get
        {
            if (this.RowId == HardcodedItems.FreeCompanyCreditItemId)
            {
                return "fccredit";
            }

            return this.RowId.ToString();
        }
    }
}