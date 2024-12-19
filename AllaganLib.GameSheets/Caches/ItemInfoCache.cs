using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Helpers;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Caches;

public class ItemInfoCache
{
    private readonly SheetManager sheetManager;
    private readonly SheetIndexer sheetIndexer;
    private readonly GameData gameData;
    private readonly List<StoreItem> storeItems;
    private readonly List<MobDrop> mobDrops;
    private readonly List<MobSpawnPosition> mobSpawnPositions;
    private readonly List<FateItem> fateItems;
    private readonly List<ItemSupplement> itemSupplements;
    private readonly List<SubmarineDrop> submarineDrops;
    private readonly List<AirshipDrop> airshipDrops;
    private readonly List<DungeonChest> dungeonChests;
    private readonly List<DungeonChestItem> dungeonChestItems;
    private readonly List<DungeonBoss> dungeonBosses;
    private readonly List<DungeonBossChest> dungeonBossChests;
    private readonly List<DungeonDrop> dungeonDrops;
    private readonly List<DungeonBossDrop> dungeonBossDrops;

    private Dictionary<uint, List<ItemSource>> itemSources;
    private Dictionary<uint, List<ItemSource>> itemUses;
    private Dictionary<(uint, uint), List<ItemSource>> itemSourceUseMap;

    private Dictionary<uint, Dictionary<ItemInfoType, HashSet<uint>>> itemSourceMapLocations;
    private Dictionary<uint, Dictionary<ItemInfoType, HashSet<uint>>> itemUseMapLocations;

    private Dictionary<uint, List<IShop>> eNpcIdToIShopLookup;

    private Dictionary<ItemInfoType, List<ItemSource>>? itemSourcesByType;
    private Dictionary<ItemInfoType, List<ItemSource>>? itemUsesByType;

    private Dictionary<ItemInfoType, HashSet<uint>>? itemSourceIdsByType;
    private Dictionary<ItemInfoType, HashSet<uint>>? itemUseIdsByType;

    public ItemInfoCache(
        SheetManager sheetManager,
        SheetIndexer sheetIndexer,
        GameData gameData,
        List<StoreItem> storeItems,
        List<MobDrop> mobDrops,
        List<MobSpawnPosition> mobSpawnPositions,
        List<FateItem> fateItems,
        List<ItemSupplement> itemSupplements,
        List<SubmarineDrop> submarineDrops,
        List<AirshipDrop> airshipDrops,
        List<DungeonChest> dungeonChests,
        List<DungeonChestItem> dungeonChestItems,
        List<DungeonBoss> dungeonBosses,
        List<DungeonBossChest> dungeonBossChests,
        List<DungeonDrop> dungeonDrops,
        List<DungeonBossDrop> dungeonBossDrop)
    {
        this.sheetManager = sheetManager;
        this.sheetIndexer = sheetIndexer;
        this.gameData = gameData;
        this.storeItems = storeItems;
        this.mobDrops = mobDrops;
        this.mobSpawnPositions = mobSpawnPositions;
        this.fateItems = fateItems;
        this.itemSupplements = itemSupplements;
        this.submarineDrops = submarineDrops;
        this.airshipDrops = airshipDrops;
        this.dungeonChests = dungeonChests;
        this.dungeonChestItems = dungeonChestItems;
        this.dungeonBosses = dungeonBosses;
        this.dungeonBossChests = dungeonBossChests;
        this.dungeonDrops = dungeonDrops;
        this.dungeonBossDrops = dungeonBossDrop;
        this.itemSources = new Dictionary<uint, List<ItemSource>>();
        this.itemUses = new Dictionary<uint, List<ItemSource>>();
        this.itemSourceUseMap = new Dictionary<(uint, uint), List<ItemSource>>();
        this.eNpcIdToIShopLookup = new Dictionary<uint, List<IShop>>();
        this.itemSourceMapLocations = new();
        this.itemUseMapLocations = new();
    }

    public Dictionary<ItemInfoType, HashSet<uint>> GetItemSourceMapLocationsByItemId(uint itemId)
    {
        return this.itemSourceMapLocations.TryGetValue(itemId, out var result) ? result : new Dictionary<ItemInfoType, HashSet<uint>>();
    }

    public Dictionary<ItemInfoType, HashSet<uint>> GetItemUseMapLocationsByItemId(uint itemId)
    {
        return this.itemUseMapLocations.TryGetValue(itemId, out var result) ? result : new Dictionary<ItemInfoType, HashSet<uint>>();
    }

    public List<ItemSource> GetItemSourcesByType(ItemInfoType type)
    {
        this.itemSourcesByType ??= this.itemSources.SelectMany(c => c.Value).GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.ToList());
        if (this.itemSourcesByType.TryGetValue(type, out var byType))
        {
            return byType;
        }

        return [];
    }

    public List<T> GetItemSourcesByType<T>(ItemInfoType type)
        where T : ItemSource
    {
        this.itemSourcesByType ??= this.itemSources.SelectMany(c => c.Value).GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.ToList());
        if (this.itemSourcesByType.TryGetValue(type, out var byType))
        {
            return byType.Cast<T>().ToList();
        }

        return [];
    }

    public HashSet<uint> GetItemSourceIdsByType(ItemInfoType type)
    {
        this.itemSourceIdsByType ??= this.itemSources.SelectMany(c => c.Value).GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.Select(d => d.Item.RowId).Distinct().ToHashSet());
        if (this.itemSourceIdsByType.TryGetValue(type, out var byType))
        {
            return byType;
        }

        return [];
    }

    public HashSet<uint> GetItemUseIdsByType(ItemInfoType type)
    {
        this.itemUseIdsByType ??= this.itemUses.SelectMany(c => c.Value).GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.Select(d => d.Item.RowId).Distinct().ToHashSet());
        if (this.itemUseIdsByType.TryGetValue(type, out var byType))
        {
            return byType;
        }

        return [];
    }


    public HashSet<uint> GetItemSourceIdsByCategory(ItemInfoCategory category)
    {
        this.itemSourceIdsByType ??= this.itemSources.SelectMany(c => c.Value).GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.Select(d => d.Item.RowId).Distinct().ToHashSet());
        return this.itemSourceIdsByType.Where(c => c.Key.InCategory(category)).SelectMany(c => c.Value).ToHashSet();
    }

    public HashSet<uint> GetItemUseIdsByCategory(ItemInfoCategory category)
    {
        this.itemUseIdsByType ??= this.itemUses.SelectMany(c => c.Value).GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.Select(d => d.Item.RowId).Distinct().ToHashSet());
        return this.itemUseIdsByType.Where(c => c.Key.InCategory(category)).SelectMany(c => c.Value).ToHashSet();
    }

    public List<ItemSource> GetItemUsesByType(ItemInfoType type)
    {
        this.itemUsesByType ??= this.itemUses.SelectMany(c => c.Value).GroupBy(c => c.Type).ToDictionary(c => c.Key, c => c.ToList());
        if (this.itemUsesByType.TryGetValue(type, out var byType))
        {
            return byType;
        }

        return [];
    }

    public List<ItemSource>? GetItemSources(uint itemId, uint? sourceId = null)
    {
        if (sourceId != null)
        {
            return this.GetItemSources(itemId, sourceId.Value);
        }

        return this.itemSources.GetValueOrDefault(itemId);
    }

    public List<T>? GetItemSources<T>(uint itemId, ItemInfoType itemInfoType)
    {
        return this.itemSources.GetValueOrDefault(itemId)?.Where(c => c.Type == itemInfoType).OfType<T>().ToList();
    }

    public bool HasItemSources(uint itemId, ItemInfoType itemInfoType)
    {
        return this.itemSources.GetValueOrDefault(itemId)?.Any(c => c.Type == itemInfoType) ?? false;
    }

    public bool HasItemSources(uint itemId, ItemInfoCategory itemInfoCategory)
    {
        return this.itemSources.GetValueOrDefault(itemId)?.Select(c => c.Type).InCategory(itemInfoCategory) ?? false;
    }

    public bool HasItemUses(uint itemId, ItemInfoType itemInfoType)
    {
        return this.itemUses.GetValueOrDefault(itemId)?.Any(c => c.Type == itemInfoType) ?? false;
    }

    public bool HasItemUses(uint itemId, ItemInfoCategory itemInfoCategory)
    {
        return this.itemUses.GetValueOrDefault(itemId)?.Select(c => c.Type).InCategory(itemInfoCategory) ?? false;
    }

    public List<ItemSource>? GetItemUses(uint itemId)
    {
        return this.itemUses.GetValueOrDefault(itemId);
    }

    public List<ItemSource>? GetItemSources(uint itemId, uint sourceId)
    {
        return this.itemSourceUseMap.GetValueOrDefault((itemId, sourceId));
    }

    public List<IShop>? GetNpcShops(uint npcId)
    {
        return this.eNpcIdToIShopLookup.GetValueOrDefault(npcId);
    }

    public Dictionary<uint, List<IShop>>? GetNpcShops()
    {
        return this.eNpcIdToIShopLookup;
    }

    public void BuildCache()
    {
        var recipeSheet = this.sheetManager.GetSheet<RecipeSheet>();
        var companyCraftSequenceSheet = this.sheetManager.GetSheet<CompanyCraftSequenceSheet>();
        var itemSheet = this.sheetManager.GetSheet<ItemSheet>();
        var gilShops = this.sheetManager.GetSheet<GilShopSheet>();
        var fccShops = this.sheetManager.GetSheet<FccShopSheet>();
        var specialShops = this.sheetManager.GetSheet<SpecialShopSheet>();
        var gcShops = this.sheetManager.GetSheet<GCShopSheet>();
        var fittingShopItemSetSheet = this.sheetManager.GetSheet<FittingShopItemSetSheet>();
        var gatheringItemSheet = this.sheetManager.GetSheet<GatheringItemSheet>();
        var bNpcNameSheet = this.sheetManager.GetSheet<BNpcNameSheet>();
        var fishingSpotSheet = this.sheetManager.GetSheet<FishingSpotSheet>();
        var fishParameterSheet = this.sheetManager.GetSheet<FishParameterSheet>();
        var spearfishingSheet = this.sheetManager.GetSheet<SpearfishingItemSheet>();
        var fateSheet = this.sheetManager.GetSheet<FateSheet>();
        var retainerTaskSheet = this.sheetManager.GetSheet<RetainerTaskSheet>();
        var airshipExplorationPointSheet = this.sheetManager.GetSheet<AirshipExplorationPointSheet>();
        var submarineExplorationPointSheet = this.sheetManager.GetSheet<SubmarineExplorationSheet>();
        var contentFinderConditionSheet = this.sheetManager.GetSheet<ContentFinderConditionSheet>();
        var satisfactionSupplySheet = this.sheetManager.GetSheet<SatisfactionSupplySheet>();
        var hwdGathererInspectionSheet = this.sheetManager.GetSheet<HWDGathererInspectionSheet>();
        var aquariumFishSheet = this.sheetManager.GetSheet<AquariumFishSheet>();
        var gcSupplyDutySheet = this.sheetManager.GetSheet<GCSupplyDutySheet>();
        var hwdCrafterSupplySheet = this.sheetManager.GetSheet<HWDCrafterSupplySheet>();
        var craftLeveSheet = this.sheetManager.GetSheet<CraftLeveSheet>();
        var cabinetSheet = this.sheetManager.GetSheet<CabinetSheet>();
        var gcSupplyDutyRewardSheet = this.sheetManager.GetSheet<GCSupplyDutyRewardSheet>();
        var stainSheet = this.gameData.GetExcelSheet<Stain>()!;
        var stainTransientSheet = this.gameData.GetExcelSheet<StainTransient>()!;
        var achievementSheet = this.gameData.GetExcelSheet<Achievement>()!;
        var buddyItemSheet = this.gameData.GetExcelSheet<BuddyItem>()!;
        var furnitureCatalogSheet = this.gameData.GetExcelSheet<FurnitureCatalogItemList>()!;
        var houseYardObjectSheet = this.gameData.GetExcelSheet<HousingYardObject>()!;
        var housingPresetSheet = this.gameData.GetExcelSheet<HousingPreset>()!;
        var mirageStoreSetItemSheet = this.gameData.GetExcelSheet<MirageStoreSetItem>()!;
        var companyCraftDraftSheet = this.gameData.GetExcelSheet<CompanyCraftDraft>()!;

        var rewards = gcSupplyDutyRewardSheet.ToDictionary(c => c.RowId, c => c);
        var maxiLevel = rewards.Select(c => c.Key).Max();

        foreach (var item in itemSheet)
        {
            if (item.Base.LevelItem.RowId <= maxiLevel && item.Base.PriceLow != 0 && item.CanTryOn &&
                item.Base.Rarity is 2 or 3 or 7)
            {
                if (rewards.TryGetValue(item.Base.LevelItem.RowId, out var reward))
                {
                    var source = new ItemGCExpertDeliverySource(item, reward);
                    this.AddItemUse(source);
                }
            }
        }

        foreach (var stain in stainSheet)
        {
            if (stain.RowId == 0)
            {
                continue;
            }

            var stainTransient = stainTransientSheet.GetRow(stain.RowId);

            if (stainTransient.Item1.IsValid && stainTransient.Item1.RowId != 0)
            {
                var item = itemSheet.GetRowOrDefault(stainTransient.Item1.RowId);

                if (item != null)
                {
                    var source = new ItemStainSource(item, new RowRef<Stain>(this.gameData.Excel, stain.RowId));
                    this.AddItemUse(source);
                }
            }

            if (stainTransient.Item2.IsValid && stainTransient.Item2.RowId != 0)
            {
                var item = itemSheet.GetRowOrDefault(stainTransient.Item2.RowId);

                if (item != null)
                {
                    var source = new ItemStainSource(item, new RowRef<Stain>(this.gameData.Excel, stain.RowId));
                    this.AddItemUse(source);
                }
            }
        }

        foreach (var furnitureCatalogItem in furnitureCatalogSheet)
        {
            if (!furnitureCatalogItem.Item.IsValid || furnitureCatalogItem.Item.RowId == 0)
            {
                continue;
            }

            var item = itemSheet.GetRowOrDefault(furnitureCatalogItem.Item.RowId);

            if (item != null)
            {
                var source = new ItemFurnitureSource(item, new RowRef<FurnitureCatalogItemList>(this.gameData.Excel, furnitureCatalogItem.RowId));
                this.AddItemUse(source);
            }
        }

        foreach (var housingYardObject in houseYardObjectSheet)
        {
            if (!housingYardObject.Item.IsValid || housingYardObject.Item.RowId == 0)
            {
                continue;
            }

            var item = itemSheet.GetRowOrDefault(housingYardObject.Item.RowId);

            if (item != null)
            {
                var source = new ItemExteriorFurnitureSource(item, new RowRef<HousingYardObject>(this.gameData.Excel, housingYardObject.RowId));
                this.AddItemUse(source);
            }
        }

        foreach (var housingPreset in housingPresetSheet)
        {
            if (housingPreset.ExteriorRoof.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.ExteriorRoof.RowId);
                var source = new ItemHouseRoofSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.ExteriorWall.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.ExteriorWall.RowId);
                var source = new ItemHouseWallSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.ExteriorWindow.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.ExteriorWindow.RowId);
                var source = new ItemHouseWindowSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.ExteriorDoor.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.ExteriorDoor.RowId);
                var source = new ItemHouseDoorSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.InteriorWall.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.InteriorWall.RowId);
                var source = new ItemHouseWallpaperSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.InteriorFlooring.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.InteriorFlooring.RowId);
                var source = new ItemHouseFlooringSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.InteriorLighting.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.InteriorLighting.RowId);
                var source = new ItemHouseLightingSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.OtherFloorWall.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.OtherFloorWall.RowId);
                var source = new ItemHouseWallpaperSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.OtherFloorFlooring.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.OtherFloorFlooring.RowId);
                var source = new ItemHouseFlooringSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.OtherFloorLighting.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.OtherFloorLighting.RowId);
                var source = new ItemHouseLightingSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.BasementWall.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.BasementWall.RowId);
                var source = new ItemHouseWallpaperSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.BasementFlooring.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.BasementFlooring.RowId);
                var source = new ItemHouseFlooringSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.BasementLighting.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.BasementLighting.RowId);
                var source = new ItemHouseLightingSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }

            if (housingPreset.MansionLighting.RowId != 0)
            {
                var item = itemSheet.GetRow(housingPreset.MansionLighting.RowId);
                var source = new ItemHouseLightingSource(new RowRef<HousingPreset>(this.gameData.Excel, housingPreset.RowId), item);
                this.AddItemUse(source);
            }
        }

        foreach (var achievement in achievementSheet)
        {
            if (!achievement.Item.IsValid || achievement.Item.RowId == 0)
            {
                continue;
            }

            var item = itemSheet.GetRowOrDefault(achievement.Item.RowId);

            if (item != null)
            {
                var source = new ItemAchievementSource(item, new RowRef<Achievement>(this.gameData.Excel, achievement.RowId));
                this.AddItemSource(source);
            }
        }

        Dictionary<(uint, uint), HashSet<uint>> setItems = new();

        foreach (var mirageStoreSetItem in mirageStoreSetItemSheet)
        {
            List<uint> ids = new List<uint>
                {
                    mirageStoreSetItem.Unknown0, mirageStoreSetItem.Unknown1, mirageStoreSetItem.Unknown2,
                    mirageStoreSetItem.Unknown3, mirageStoreSetItem.Unknown4, mirageStoreSetItem.Unknown5,
                    mirageStoreSetItem.Unknown6, mirageStoreSetItem.Unknown7, mirageStoreSetItem.Unknown8,
                    mirageStoreSetItem.Unknown9, mirageStoreSetItem.Unknown10,
                };
            ids = ids.Where(c => c != 0).ToList();

            foreach (var mainItem in ids)
            {
                foreach (var secondaryItem in ids)
                {
                    setItems.TryAdd((mirageStoreSetItem.RowId, mainItem), new HashSet<uint>());
                    setItems[(mirageStoreSetItem.RowId, mainItem)].Add(secondaryItem);
                }
            }
        }

        foreach (var setItem in setItems)
        {
            var source = new ItemGlamourReadySetItemSource(itemSheet.GetRow(setItem.Key.Item2), itemSheet.GetRow(setItem.Key.Item1), setItem.Value.Select(c => itemSheet.GetRow(c)).ToList());
            this.AddItemUse(source);
        }


        foreach (var setItem in setItems.DistinctBy(c => c.Key.Item1))
        {
            var source = new ItemGlamourReadySetSource(itemSheet.GetRow(setItem.Key.Item1), setItem.Value.Select(c => itemSheet.GetRow(c)).ToList());
            this.AddItemUse(source);
        }

        foreach (var buddyItem in buddyItemSheet)
        {
            if (!buddyItem.Item.IsValid || buddyItem.Item.RowId == 0)
            {
                continue;
            }

            var item = itemSheet.GetRowOrDefault(buddyItem.Item.RowId);

            if (item != null)
            {
                var source = new ItemBuddySource(item, new RowRef<BuddyItem>(this.gameData.Excel, buddyItem.RowId));
                this.AddItemUse(source);
            }
        }

        foreach (var cabinet in cabinetSheet)
        {
            var item = itemSheet.GetRowOrDefault(cabinet.Base.Item.RowId);

            if (item != null)
            {
                var use = new ItemArmoireSource(cabinet, item);
                this.AddItemUse(use);
            }
        }

        foreach (var craftLeve in craftLeveSheet)
        {
            for (var index = 0; index < craftLeve.Base.Item.Count; index++)
            {
                var item = itemSheet.GetRowOrDefault(craftLeve.Base.Item[index].RowId);

                if (item != null)
                {
                    var use = new ItemCraftLeveSource(craftLeve, index, item);
                    this.AddItemUse(use);
                }
            }
        }

        foreach (var recipe in recipeSheet)
        {
            var result = itemSheet.GetRow(recipe.Base.ItemResult.RowId);
            var source = new ItemCraftResultSource(result, recipe);
            this.AddItemSource(source);
            foreach (var ingredientCount in recipe.IngredientCounts)
            {
                var ingredientItem = itemSheet.GetRow(ingredientCount.Key);
                var use = new ItemCraftRequirementSource(result, ingredientItem, recipe);
                this.AddItemUse(use);
                this.AddItemSourceUseCombo(source, use);
            }
        }

        foreach (var companyCraftSequence in companyCraftSequenceSheet)
        {
            if (companyCraftSequence.Base.ResultItem.RowId == 0)
            {
                continue;
            }

            var item = itemSheet.GetRowOrDefault(companyCraftSequence.Base.ResultItem.RowId);
            if (item == null)
            {
                continue;
            }

            var source = new ItemCompanyCraftResultSource(item, companyCraftSequence);
            this.AddItemSource(source);
            foreach (var requiredItem in companyCraftSequence.MaterialsRequired(null))
            {
                var costItem = itemSheet.GetRowOrDefault(requiredItem.ItemId);
                if (costItem == null)
                {
                    continue;
                }
                var use = new ItemCompanyCraftRequirementSource(item, costItem, requiredItem, companyCraftSequence);
                this.AddItemUse(use);
                this.AddItemSourceUseCombo(source, use);
            }
        }

        foreach (var companyCraftDraft in companyCraftDraftSheet)
        {
            for (var index = 0; index < companyCraftDraft.RequiredItem.Count; index++)
            {
                var itemRef = companyCraftDraft.RequiredItem[index];
                var requiredAmount = companyCraftDraft.RequiredItemCount[index];
                if (itemRef.RowId == 0)
                {
                    continue;
                }
                var item = itemSheet.GetRow(itemRef.RowId);
                var use = new ItemCompanyCraftDraftSource(item, new RowRef<CompanyCraftDraft>(this.gameData.Excel, companyCraftDraft.RowId, this.gameData.Options.DefaultExcelLanguage), requiredAmount);
                this.AddItemUse(use);
            }
        }

        foreach (var gilShop in gilShops)
        {
            var isCalamitySalvager = gilShop.ENpcs.Any(c => c.IsCalamitySalvager);

            var mapIds = gilShop.MapIds;

            foreach (var npc in gilShop.ENpcs)
            {
                this.AddShopNpcLookup(gilShop, npc.RowId);
            }

            foreach (var gilShopItem in gilShop.GilShopItems)
            {
                var source = isCalamitySalvager ? new ItemCalamitySalvagerShopSource(gilShopItem, gilShop) : new ItemGilShopSource(gilShopItem, gilShop);
                this.AddItemSource(source);
                this.AddItemUse(source);
                this.AddItemSourceUseCombo(source, source);

                this.AddItemSourceMapLocation(gilShopItem.Item.RowId, mapIds, isCalamitySalvager ? ItemInfoType.CalamitySalvagerShop : ItemInfoType.GilShop);
                this.AddItemUseMapLocation(gilShopItem.Costs.First().Item.RowId, mapIds, isCalamitySalvager ? ItemInfoType.CalamitySalvagerShop : ItemInfoType.GilShop);
            }
        }

        foreach (var fccShop in fccShops)
        {
            var mapIds = fccShop.MapIds;

            foreach (var npc in fccShop.ENpcs)
            {
                this.AddShopNpcLookup(fccShop, npc.RowId);
            }

            foreach (var fccShopListing in fccShop.FccShopListings)
            {
                var source = new ItemFccShopSource(fccShopListing, fccShop);
                this.AddItemSource(source);
                this.AddItemUse(source);
                this.AddItemSourceUseCombo(source, source);

                this.AddItemSourceMapLocation(fccShopListing.Reward.Item.RowId, mapIds, ItemInfoType.FCShop);
                this.AddItemUseMapLocation(fccShopListing.Costs.First().Item.RowId, mapIds, ItemInfoType.FCShop);
            }
        }

        foreach (var specialShop in specialShops)
        {
            var mapIds = specialShop.MapIds;

            foreach (var npc in specialShop.ENpcs)
            {
                this.AddShopNpcLookup(specialShop, npc.RowId);
            }

            foreach (var specialShopListing in specialShop.SpecialShopListings)
            {
                // Fix for blank items that SQ seems to have added
                if (specialShopListing.Rewards.All(c => c.Item.Icon == 0))
                {
                    continue;
                }

                List<ItemSource> sources = new();
                foreach (var shopListingItem in specialShopListing.Rewards)
                {
                    if (specialShop is { IsFateShop: true, FateShop: not null })
                    {
                        var source = new ItemFateShopSource(shopListingItem, specialShopListing, specialShop.FateShop, specialShop);
                        this.AddItemSource(source);
                        this.AddItemSourceMapLocation(source.Item.RowId, mapIds, ItemInfoType.FateShop);
                        sources.Add(source);

                        foreach (var cost in specialShopListing.Costs)
                        {
                            var use = new ItemFateShopSource(cost, specialShopListing, specialShop.FateShop, specialShop);
                            this.AddItemUse(use);
                            this.AddItemUseMapLocation(cost.Item.RowId, mapIds, ItemInfoType.FateShop);
                            this.AddItemSourceUseCombo(source, use);
                        }
                    }
                    else
                    {
                        var source = new ItemSpecialShopSource(shopListingItem.Item, null, shopListingItem, specialShopListing, specialShop);
                        this.AddItemSource(source);
                        this.AddItemSourceMapLocation(shopListingItem.Item.RowId, mapIds, ItemInfoType.SpecialShop);
                        sources.Add(source);

                        foreach (var cost in specialShopListing.Costs)
                        {
                            var use = new ItemSpecialShopSource(shopListingItem.Item, cost.Item, shopListingItem, specialShopListing, specialShop);
                            this.AddItemUse(use);
                            this.AddItemUseMapLocation(cost.Item.RowId, mapIds, ItemInfoType.SpecialShop);
                            this.AddItemSourceUseCombo(source, use);
                        }
                    }
                }
            }
        }

        foreach (var gcShop in gcShops)
        {
            var mapIds = gcShop.MapIds;

            foreach (var npc in gcShop.ENpcs)
            {
                this.AddShopNpcLookup(gcShop, npc.RowId);
            }

            foreach (var gcShopListing in gcShop.GCShopListings)
            {
                var source = new ItemGCShopSource(gcShopListing, gcShop);
                this.AddItemSource(source);
                this.AddItemUse(source);
                this.AddItemSourceUseCombo(source, source);

                this.AddItemSourceMapLocation(gcShopListing.Item.RowId, mapIds, ItemInfoType.GCShop);
                this.AddItemUseMapLocation(gcShopListing.Costs.First().Item.RowId, mapIds, ItemInfoType.GCShop);
            }
        }



        foreach (var storeItem in this.storeItems)
        {
            var item = itemSheet.GetRowOrDefault(storeItem.ItemId);
            var fittingShopItemSetRow = fittingShopItemSetSheet.GetRowOrDefault(storeItem.FittingShopItemSetId);
            if (item != null)
            {
                this.AddItemSource(new ItemCashShopSource(item, storeItem, fittingShopItemSetRow));
            }
        }

        foreach (var mobDrop in this.mobDrops)
        {
            var item = itemSheet.GetRowOrDefault(mobDrop.ItemId);
            var bNpcNameRow = bNpcNameSheet.GetRowOrDefault(mobDrop.BNpcNameId);

            if (item != null && bNpcNameRow != null)
            {
                var mapIds = bNpcNameRow.MapIds;

                this.AddItemSource(new ItemMonsterDropSource(item, bNpcNameRow, mobDrop));
                this.AddItemSourceMapLocation(mobDrop.Item.RowId, mapIds, ItemInfoType.Monster);
            }
        }

        foreach (var fishParameter in fishParameterSheet)
        {
            var fishingSpots = fishParameter.FishingSpots;
            var item = itemSheet.GetRowOrDefault(fishParameter.Base.Item.RowId);
            if (item != null)
            {
                this.AddItemSource(new ItemFishingSource(fishParameter, fishingSpots, item));
                var mapIds = fishingSpots.Select(c => c.Base.TerritoryType.ValueNullable?.Map.RowId ?? 0).Where(c => c != 0).Distinct();
                foreach (var mapId in mapIds)
                {
                    this.AddItemSourceMapLocation(item.RowId, mapId, ItemInfoType.Fishing);
                }
            }
        }

        foreach (var spearfishing in spearfishingSheet)
        {
            if (spearfishing.ItemRow != null)
            {
                var gatheringPoints = spearfishing.GatheringPoints;
                this.AddItemSource(new ItemSpearfishingSource(spearfishing));
                foreach (var gatheringPoint in gatheringPoints)
                {
                    if (gatheringPoint.SpearfishingNotebook is { TerritoryTypeRow.Map: not null })
                    {
                        this.AddItemSourceMapLocation(spearfishing.ItemRow.RowId, gatheringPoint.SpearfishingNotebook.TerritoryTypeRow.Map.RowId, ItemInfoType.Spearfishing);
                    }
                }
            }
        }

        foreach (var fateItem in this.fateItems)
        {
            if (fateItem.Item.RowId != 0)
            {
                var item = itemSheet.GetRowOrDefault(fateItem.ItemId);
                var fate = fateSheet.GetRowOrDefault(fateItem.FateId);

                if (item != null && fate != null)
                {
                    this.AddItemSource(new ItemFateSource(item, fate));
                }
            }
        }

        foreach (var airshipDrop in this.airshipDrops)
        {
            if (airshipDrop.Item.RowId != 0)
            {
                var item = itemSheet.GetRowOrDefault(airshipDrop.ItemId);
                var airshipExplorationPoint = airshipExplorationPointSheet.GetRowOrDefault(airshipDrop.AirshipExplorationPointId);

                if (item != null && airshipExplorationPoint != null)
                {
                    this.AddItemSource(new ItemAirshipDropSource(item, airshipExplorationPoint));
                }
            }
        }

        foreach (var submarineDrop in this.submarineDrops)
        {
            if (submarineDrop.Item.RowId != 0)
            {
                var item = itemSheet.GetRowOrDefault(submarineDrop.ItemId);
                var submarineExplorationPoint = submarineExplorationPointSheet.GetRowOrDefault(submarineDrop.SubmarineExplorationId);

                if (item != null && submarineExplorationPoint != null)
                {
                    this.AddItemSource(new ItemSubmarineDropSource(item, submarineExplorationPoint));
                }
            }
        }

        var ventureItem = itemSheet.GetRow(HardcodedItems.VentureId);

        foreach (var retainerTask in retainerTaskSheet)
        {
            foreach (var drop in retainerTask.Drops)
            {
                switch (retainerTask.RetainerTaskType)
                {
                    case RetainerTaskType.HighlandExploration:
                        this.AddItemSource(new ItemHighlandExplorationVentureSource(drop, ventureItem, retainerTask));
                        break;
                    case RetainerTaskType.WoodlandExploration:
                        this.AddItemSource(new ItemWoodlandExplorationVentureSource(drop, ventureItem, retainerTask));
                        break;
                    case RetainerTaskType.WatersideExploration:
                        this.AddItemSource(new ItemWatersideExplorationVentureSource(drop, ventureItem, retainerTask));
                        break;
                    case RetainerTaskType.FieldExploration:
                        this.AddItemSource(new ItemFieldExplorationVentureSource(drop, ventureItem, retainerTask));
                        break;
                    case RetainerTaskType.QuickExploration:
                        this.AddItemSource(new ItemQuickVentureSource(drop, ventureItem, retainerTask));
                        break;
                    case RetainerTaskType.Hunting:
                        this.AddItemSource(new ItemHuntingVentureSource(drop, ventureItem, retainerTask));
                        break;
                    case RetainerTaskType.Mining:
                        this.AddItemSource(new ItemMiningVentureSource(drop, ventureItem, retainerTask));
                        break;
                    case RetainerTaskType.Botanist:
                        this.AddItemSource(new ItemBotanistVentureSource(drop, ventureItem, retainerTask));
                        break;
                    case RetainerTaskType.Fishing:
                        this.AddItemSource(new ItemFishingVentureSource(drop, ventureItem, retainerTask));
                        break;
                }
            }
        }

        var groupedDungeonChestItems = this.dungeonChestItems.GroupBy(c => c.ChestId).ToDictionary(c => c.Key, c => c.ToList());

        foreach (var dungeonChest in this.dungeonChests)
        {
            if (!groupedDungeonChestItems.ContainsKey(dungeonChest.RowId))
            {
                continue;
            }

            var contentFinderConditionRow = contentFinderConditionSheet.GetRowOrDefault(dungeonChest.ContentFinderConditionId);

            if (contentFinderConditionRow == null)
            {
                continue;
            }


            var chestItems = groupedDungeonChestItems[dungeonChest.RowId];

            foreach (var dungeonChestItem in chestItems)
            {
                if (dungeonChestItem.ItemId == 0)
                {
                    continue;
                }

                var item = itemSheet.GetRowOrDefault(dungeonChestItem.ItemId);

                if (item != null)
                {
                    this.AddItemSource(new ItemDungeonChestSource(item, contentFinderConditionRow, dungeonChestItem, dungeonChest));
                }
            }
        }

        var dungeonBossesByIds = this.dungeonBosses.DistinctBy(c => (c.ContentFinderConditionId, c.FightNo)).ToDictionary(c => (c.ContentFinderConditionId, c.FightNo), c => c);

        foreach (var dungeonBossDrop in this.dungeonBossDrops)
        {
            var tuple = (dungeonBossDrop.ContentFinderConditionId, dungeonBossDrop.FightNo);
            if (!dungeonBossesByIds.TryGetValue(tuple, out var dungeonBoss))
            {
                continue;
            }

            var item = itemSheet.GetRowOrDefault(dungeonBossDrop.ItemId);
            var contentFinderCondition = contentFinderConditionSheet.GetRowOrDefault(dungeonBossDrop.ContentFinderConditionId);
            var bNpcName = bNpcNameSheet.GetRowOrDefault(dungeonBoss.BNpcNameId);

            if (item == null || contentFinderCondition == null || bNpcName == null)
            {
                continue;
            }

            this.AddItemSource(new ItemDungeonBossDropSource(item, contentFinderCondition, bNpcName, dungeonBoss, dungeonBossDrop));
        }

        foreach (var dungeonBossChest in this.dungeonBossChests)
        {
            var tuple = (dungeonBossChest.ContentFinderConditionId, dungeonBossChest.FightNo);
            if (!dungeonBossesByIds.TryGetValue(tuple, out var dungeonBoss))
            {
                continue;
            }

            var item = itemSheet.GetRowOrDefault(dungeonBossChest.ItemId);
            var contentFinderCondition = contentFinderConditionSheet.GetRowOrDefault(dungeonBossChest.ContentFinderConditionId);
            var bNpcName = bNpcNameSheet.GetRowOrDefault(dungeonBoss.BNpcNameId);

            if (item == null || contentFinderCondition == null || bNpcName == null)
            {
                continue;
            }

            this.AddItemSource(new ItemDungeonBossChestSource(item, contentFinderCondition, bNpcName, dungeonBoss, dungeonBossChest));
        }

        foreach (var dungeonDrop in this.dungeonDrops)
        {
            var item = itemSheet.GetRowOrDefault(dungeonDrop.ItemId);
            var contentFinderCondition = contentFinderConditionSheet.GetRowOrDefault(dungeonDrop.ContentFinderConditionId);

            if (item == null || contentFinderCondition == null)
            {
                continue;
            }

            this.AddItemSource(new ItemDungeonDropSource(item, contentFinderCondition, dungeonDrop));
        }

        foreach (var hwdGathererInspection in hwdGathererInspectionSheet)
        {
            for (var index = 0; index < hwdGathererInspection.Base.HWDGathererInspectionData.Count; index++)
            {
                var data = hwdGathererInspection.Base.HWDGathererInspectionData[index];

                if ((data.RequiredItem.RowId == 0 && data.FishParameter.RowId == 0) || data.ItemReceived.RowId == 0 ||
                    (data.RequiredItem.Value.Item.RowId == 0 && data.FishParameter.Value.Item.RowId == 0))
                {
                    continue;
                }

                var requiredItem = itemSheet.GetRowOrDefault(data.RequiredItem.Value.Item.RowId != 0 ? data.RequiredItem.Value.Item.RowId : data.FishParameter.Value.Item.RowId);
                var receivedItem = itemSheet.GetRowOrDefault(data.ItemReceived.RowId);

                if (requiredItem == null || receivedItem == null)
                {
                    continue;
                }

                var source = new ItemSkybuilderInspectionSource(hwdGathererInspection, index, receivedItem, requiredItem);
                this.AddItemSource(source);
                this.AddItemUse(source);
                this.AddItemSourceUseCombo(source, source);
            }
        }

        foreach (var supplementalItem in this.itemSupplements)
        {
            var item = itemSheet.GetRowOrDefault(supplementalItem.ItemId);
            var sourceItem = itemSheet.GetRowOrDefault(supplementalItem.SourceItemId);
            if (item == null || sourceItem == null)
            {
                continue;
            }

            switch (supplementalItem.ItemSupplementSource)
            {
                case LuminaSupplemental.Excel.Model.ItemSupplementSource.Desynth:
                    var source1 = new ItemDesynthSource(item, sourceItem);
                    this.AddItemSource(source1);
                    this.AddItemUse(source1);
                    this.AddItemSourceUseCombo(source1, source1);
                    break;
                case LuminaSupplemental.Excel.Model.ItemSupplementSource.Reduction:
                    var source2 = new ItemReductionSource(item, sourceItem);
                    this.AddItemSource(source2);
                    this.AddItemUse(source2);
                    this.AddItemSourceUseCombo(source2, source2);
                    break;
                case LuminaSupplemental.Excel.Model.ItemSupplementSource.Loot:
                    var source3 = new ItemLootSource(item, sourceItem);
                    this.AddItemSource(source3);
                    this.AddItemUse(source3);
                    this.AddItemSourceUseCombo(source3, source3);
                    break;
                case LuminaSupplemental.Excel.Model.ItemSupplementSource.Gardening:
                    var source4 = new ItemGardeningSource(item, sourceItem);
                    this.AddItemSource(source4);
                    this.AddItemUse(source4);
                    this.AddItemSourceUseCombo(source4, source4);
                    break;
            }
        }

        foreach (var satisfactionSupply in satisfactionSupplySheet)
        {
            var item = itemSheet.GetRowOrDefault(satisfactionSupply.Base.Item.RowId);
            if (item == null)
            {
                continue;
            }

            this.AddItemUse(new ItemCustomDeliverySource(item, satisfactionSupply));
        }

        foreach (var hwdCrafterSupplyRow in hwdCrafterSupplySheet)
        {
            for (var index = 0; index < hwdCrafterSupplyRow.Base.HWDCrafterSupplyParams.Count; index++)
            {
                var row = hwdCrafterSupplyRow.Base.HWDCrafterSupplyParams[index];
                var item = itemSheet.GetRowOrDefault(row.ItemTradeIn.RowId);
                if (item == null)
                {
                    continue;
                }

                this.AddItemUse(new ItemSkybuilderHandInSource(hwdCrafterSupplyRow, index, item));
            }
        }

        foreach (var gatheringItem in gatheringItemSheet)
        {
            if (gatheringItem.Item == null)
            {
                continue;
            }

            var mapIds = gatheringItem.MapIds;

            HashSet<uint> normalTypes = new();
            HashSet<uint> ephemeralTypes = new();
            HashSet<uint> timedTypes = new();
            HashSet<uint> hiddenTypes = new();

            // 0 = NotUsed
            // 1 = Normal (But collectable)
            // 2 = Unspoiled (Timed)
            // 3 = Gathering Leve?
            // 4 = Timed
            // 5 = Legendary (Timed)
            // 6 = Unknown
            // 7 = Diadem Normal
            // 8 = Diadem Rare

            foreach (var gatheringPoint in gatheringItem.GatheringPoints)
            {
                var gatheringType = gatheringPoint.GatheringPointBase.Base.GatheringType.RowId;

                if (gatheringPoint.Base.Type != 4 && gatheringPoint.GatheringPointTransient.EphemeralNode)
                {
                    ephemeralTypes.Add(gatheringType);
                }
                else if (gatheringPoint.Base.Type == 4 || gatheringPoint.GatheringPointTransient.TimedNode)
                {
                    timedTypes.Add(gatheringType);
                }
                else if (HardcodedItems.HiddenNodeItemIds.Contains(gatheringItem.Base.Item.RowId))
                {
                    hiddenTypes.Add(gatheringType);
                }
                else
                {
                    normalTypes.Add(gatheringType);
                }
            }

            foreach (var normalType in normalTypes)
            {
                switch (normalType)
                {
                    case 0:
                        this.AddItemSource(new ItemMiningSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.Mining);
                        continue;
                    case 1:
                        this.AddItemSource(new ItemQuarryingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.Quarrying);
                        continue;
                    case 2:
                        this.AddItemSource(new ItemLoggingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.Logging);
                        continue;
                    case 3:
                        this.AddItemSource(new ItemHarvestingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.Harvesting);
                        continue;
                }
            }

            foreach (var ephemeralType in ephemeralTypes)
            {
                switch (ephemeralType)
                {
                    case 0:
                        this.AddItemSource(new ItemEphemeralMiningSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.EphemeralMining);
                        continue;
                    case 1:
                        this.AddItemSource(new ItemEphemeralQuarryingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.EphemeralQuarrying);
                        continue;
                    case 2:
                        this.AddItemSource(new ItemEphemeralLoggingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.EphemeralLogging);
                        continue;
                    case 3:
                        this.AddItemSource(new ItemEphemeralHarvestingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.EphemeralHarvesting);
                        continue;
                }
            }

            foreach (var timedType in timedTypes)
            {
                switch (timedType)
                {
                    case 0:
                        this.AddItemSource(new ItemTimedMiningSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.TimedMining);
                        continue;
                    case 1:
                        this.AddItemSource(new ItemTimedQuarryingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.TimedQuarrying);
                        continue;
                    case 2:
                        this.AddItemSource(new ItemTimedLoggingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.TimedLogging);
                        continue;
                    case 3:
                        this.AddItemSource(new ItemTimedHarvestingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.TimedHarvesting);
                        continue;
                }
            }

            foreach (var hiddenType in hiddenTypes)
            {
                switch (hiddenType)
                {
                    case 0:
                        this.AddItemSource(new ItemHiddenMiningSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.HiddenMining);
                        continue;
                    case 1:
                        this.AddItemSource(new ItemHiddenQuarryingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.HiddenQuarrying);
                        continue;
                    case 2:
                        this.AddItemSource(new ItemHiddenLoggingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.HiddenLogging);
                        continue;
                    case 3:
                        this.AddItemSource(new ItemHiddenHarvestingSource(gatheringItem));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.HiddenHarvesting);
                        continue;
                }
            }
        }

        foreach (var aquariumRow in aquariumFishSheet)
        {
            var item = itemSheet.GetRowOrDefault(aquariumRow.Base.Item.RowId);
            if (item == null)
            {
                continue;
            }

            this.AddItemUse(new ItemAquariumSource(item, aquariumRow));
        }

        foreach (var gcSupplyDuty in gcSupplyDutySheet)
        {
            for (var index = 0; index < gcSupplyDuty.Base.SupplyData.Count; index++)
            {
                var supplyData = gcSupplyDuty.Base.SupplyData[index];
                for (var i = 0; i < supplyData.Item.Count; i++)
                {
                    var rowRef = supplyData.Item[i];
                    var count = supplyData.ItemCount[i];
                    var item = itemSheet.GetRowOrDefault(rowRef.RowId);
                    if (item == null)
                    {
                        continue;
                    }

                    this.AddItemUse(new ItemGCSupplyDutySource(gcSupplyDuty, count, item));
                }
            }
        }
    }

    public void AddItemSource(ItemSource itemSource)
    {
        this.itemSources.TryAdd(itemSource.Item.RowId, []);
        this.itemSources[itemSource.Item.RowId].Add(itemSource);
    }

    public void AddShopNpcLookup(IShop shop, uint npcId)
    {
        this.eNpcIdToIShopLookup.TryAdd(npcId, []);
        this.eNpcIdToIShopLookup[npcId].Add(shop);
    }

    public void AddItemUse(ItemSource itemSource)
    {
        this.itemUses.TryAdd(itemSource.CostItem?.RowId ?? itemSource.Item.RowId, []);
        this.itemUses[itemSource.CostItem?.RowId ?? itemSource.Item.RowId].Add(itemSource);
    }

    public void AddItemSourceUseCombo(ItemSource source, ItemSource use)
    {
        var tuple = (source.Item.RowId, use.CostItem?.RowId ?? use.Item.RowId);
        this.itemSourceUseMap.TryAdd(tuple, []);
        this.itemSourceUseMap[tuple].Add(source);
    }

    public void AddItemSourceMapLocation(uint itemId, uint mapId, ItemInfoType itemInfoType)
    {
        this.itemSourceMapLocations.TryAdd(itemId, []);
        this.itemSourceMapLocations[itemId].TryAdd(itemInfoType, new());
        this.itemSourceMapLocations[itemId][itemInfoType].Add(mapId);
    }

    public void AddItemSourceMapLocation(uint itemId, HashSet<uint> mapIds, ItemInfoType itemInfoType)
    {
        foreach (var mapId in mapIds)
        {
            this.AddItemSourceMapLocation(itemId, mapId, itemInfoType);
        }
    }

    public void AddItemUseMapLocation(uint itemId, uint mapId, ItemInfoType itemInfoType)
    {
        this.itemUseMapLocations.TryAdd(itemId, []);
        this.itemUseMapLocations[itemId].TryAdd(itemInfoType, new());
        this.itemUseMapLocations[itemId][itemInfoType].Add(mapId);
    }

    public void AddItemUseMapLocation(uint itemId, HashSet<uint> mapIds, ItemInfoType itemInfoType)
    {
        foreach (var mapId in mapIds)
        {
            this.AddItemUseMapLocation(itemId, mapId, itemInfoType);
        }
    }
}