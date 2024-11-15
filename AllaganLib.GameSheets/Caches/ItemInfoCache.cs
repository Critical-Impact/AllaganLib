using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Helpers;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Caches;

public class ItemInfoCache
{
    private readonly SheetManager sheetManager;
    private readonly SheetIndexer sheetIndexer;
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

    public ItemInfoCache(
        SheetManager sheetManager,
        SheetIndexer sheetIndexer,
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
        var spearfishingSheet = this.sheetManager.GetSheet<SpearfishingItemSheet>();
        var fateSheet = this.sheetManager.GetSheet<FateSheet>();
        var retainerTaskSheet = this.sheetManager.GetSheet<RetainerTaskSheet>();
        var airshipExplorationPointSheet = this.sheetManager.GetSheet<AirshipExplorationPointSheet>();
        var submarineExplorationPointSheet = this.sheetManager.GetSheet<SubmarineExplorationSheet>();
        var contentFinderConditionSheet = this.sheetManager.GetSheet<ContentFinderConditionSheet>();
        var satisfactionSupplySheet = this.sheetManager.GetSheet<SatisfactionSupplySheet>();
        var hwdGathererInspectionSheet = this.sheetManager.GetSheet<HWDGathererInspectionSheet>();
        var aquariumFishSheet = this.sheetManager.GetSheet<AquariumFishSheet>();
        var dailySupplyItemSheet = this.sheetManager.GetSheet<DailySupplyItemSheet>();
        var hwdCrafterSupplySheet = this.sheetManager.GetSheet<HWDCrafterSupplySheet>();
        var craftLeveSheet = this.sheetManager.GetSheet<CraftLeveSheet>();
        var cabinetSheet = this.sheetManager.GetSheet<CabinetSheet>();

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
            var source = new ItemCraftResultSource(itemSheet.GetRow(recipe.Base.ItemResult.RowId), recipe);
            this.AddItemSource(source);
            foreach (var ingredientCount in recipe.IngredientCounts)
            {
                var use = new ItemCraftRequirementSource(itemSheet.GetRow(ingredientCount.Key), recipe);
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
                var use = new ItemCompanyCraftRequirementSource(item, requiredItem, companyCraftSequence);
                this.AddItemUse(use);
                this.AddItemSourceUseCombo(source, use);
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
                var source = new ItemGilShopSource(gilShopItem, gilShop, isCalamitySalvager ? ItemInfoType.CalamitySalvagerShop : ItemInfoType.GilShop);
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
                List<ItemSource> sources = new();
                foreach (var shopListingItem in specialShopListing.Rewards)
                {
                    if (specialShop is { IsFateShop: true, FateShop: not null })
                    {
                        var source = new ItemFateShopSource(shopListingItem, specialShopListing, specialShop.FateShop, specialShop);
                        this.AddItemSource(source);
                        this.AddItemSourceMapLocation(source.Item.RowId, mapIds, ItemInfoType.FateShop);
                        sources.Add(source);
                    }
                    else
                    {
                        var source = new ItemSpecialShopSource(shopListingItem, specialShopListing, specialShop);
                        this.AddItemSource(source);
                        this.AddItemSourceMapLocation(shopListingItem.Item.RowId, mapIds, ItemInfoType.SpecialShop);
                        sources.Add(source);
                    }
                }

                foreach (var cost in specialShopListing.Costs)
                {
                    if (specialShop is { IsFateShop: true, FateShop: not null })
                    {
                        var use = new ItemFateShopSource(cost, specialShopListing, specialShop.FateShop, specialShop);
                        this.AddItemUse(use);
                        this.AddItemUseMapLocation(cost.Item.RowId, mapIds, ItemInfoType.FateShop);
                        foreach (var source in sources)
                        {
                            this.AddItemSourceUseCombo(source, use);
                        }
                    }
                    else
                    {
                        var use = new ItemSpecialShopSource(cost, specialShopListing, specialShop);
                        this.AddItemUse(use);
                        this.AddItemUseMapLocation(cost.Item.RowId, mapIds, ItemInfoType.SpecialShop);
                        foreach (var source in sources)
                        {
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
                this.AddItemSource(new ItemCashShopSource(item, fittingShopItemSetRow));
            }
        }

        foreach (var mobDrop in this.mobDrops)
        {
            var item = itemSheet.GetRowOrDefault(mobDrop.ItemId);
            var bNpcNameRow = bNpcNameSheet.GetRowOrDefault(mobDrop.BNpcNameId);

            if (item != null && bNpcNameRow != null)
            {
                var mapIds = bNpcNameRow.MapIds;

                this.AddItemSource(new ItemMonsterDropSource(item, bNpcNameRow));
                this.AddItemSourceMapLocation(mobDrop.Item.RowId, mapIds, ItemInfoType.Monster);
            }
        }

        foreach (var fishingSpot in fishingSpotSheet)
        {
            var mapId = fishingSpot.Base.TerritoryType.ValueNullable?.Map.RowId ?? 0;

            foreach (var item in fishingSpot.Items)
            {
                this.AddItemSource(new ItemFishingSource(fishingSpot, item));
                if (mapId != 0)
                {
                    this.AddItemSourceMapLocation(item.RowId, mapId, ItemInfoType.Fishing);
                }
            }
        }

        foreach (var spearfishing in spearfishingSheet)
        {
            var mapId = spearfishing.Base.TerritoryType.ValueNullable?.Map.RowId ?? 0;

            if (spearfishing.ItemRow != null)
            {
                this.AddItemSource(new ItemSpearfishingSource(spearfishing));
                if (mapId != 0)
                {
                    this.AddItemSourceMapLocation(spearfishing.ItemRow.RowId, mapId, ItemInfoType.Spearfishing);
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

        foreach (var retainerTask in retainerTaskSheet)
        {
            foreach (var drop in retainerTask.Drops)
            {
                switch (retainerTask.RetainerTaskType)
                {
                    case RetainerTaskType.HighlandExploration:
                        this.AddItemSource(new ItemExplorationVentureSource(drop, retainerTask, ItemInfoType.MiningExplorationVenture));
                        break;
                    case RetainerTaskType.WoodlandExploration:
                        this.AddItemSource(new ItemExplorationVentureSource(drop, retainerTask, ItemInfoType.BotanyExplorationVenture));
                        break;
                    case RetainerTaskType.WatersideExploration:
                        this.AddItemSource(new ItemExplorationVentureSource(drop, retainerTask, ItemInfoType.FishingExplorationVenture));
                        break;
                    case RetainerTaskType.FieldExploration:
                        this.AddItemSource(new ItemExplorationVentureSource(drop, retainerTask, ItemInfoType.CombatExplorationVenture));
                        break;
                    case RetainerTaskType.QuickExploration:
                        this.AddItemSource(new ItemQuickVentureSource(drop, retainerTask));
                        break;
                    case RetainerTaskType.Hunting:
                        this.AddItemSource(new ItemVentureSource(drop, retainerTask, ItemInfoType.CombatVenture));
                        break;
                    case RetainerTaskType.Mining:
                        this.AddItemSource(new ItemVentureSource(drop, retainerTask, ItemInfoType.MiningVenture));
                        break;
                    case RetainerTaskType.Botanist:
                        this.AddItemSource(new ItemVentureSource(drop, retainerTask, ItemInfoType.BotanyVenture));
                        break;
                    case RetainerTaskType.Fishing:
                        this.AddItemSource(new ItemVentureSource(drop, retainerTask, ItemInfoType.FishingVenture));
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
                //TODO: maybe warn the user or something or show in dev builds
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
                //TODO: maybe warn the user or something or show in dev builds
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

                if (data.RequiredItem.RowId == 0 || data.ItemReceived.RowId == 0 ||
                    data.RequiredItem.Value.Item.RowId == 0)
                {
                    continue;
                }

                var requiredItem = itemSheet.GetRowOrDefault(data.RequiredItem.Value.Item.RowId);
                var receivedItem = itemSheet.GetRowOrDefault(data.ItemReceived.RowId);

                if (requiredItem == null || receivedItem == null)
                {
                    continue;
                }

                var source = new ItemSkybuilderInspectionSource(hwdGathererInspection, index, requiredItem, receivedItem);
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

            foreach (var gatheringPoint in gatheringItem.GatheringPoints)
            {
                var gatheringType = gatheringPoint.GatheringPointBase.Base.GatheringType.RowId;

                if (gatheringPoint.GatheringPointTransient.EphemeralNode)
                {
                    ephemeralTypes.Add(gatheringType);
                }
                else if (gatheringPoint.GatheringPointTransient.TimedNode)
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
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.Mining));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.Mining);
                        continue;
                    case 1:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.Quarrying));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.Quarrying);
                        continue;
                    case 2:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.Logging));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.Logging);
                        continue;
                    case 3:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.Harvesting));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.Harvesting);
                        continue;
                }
            }

            foreach (var ephemeralType in ephemeralTypes)
            {
                switch (ephemeralType)
                {
                    case 0:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.EphemeralMining));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.EphemeralMining);
                        continue;
                    case 1:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.EphemeralQuarrying));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.EphemeralQuarrying);
                        continue;
                    case 2:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.EphemeralLogging));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.EphemeralLogging);
                        continue;
                    case 3:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.EphemeralHarvesting));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.EphemeralHarvesting);
                        continue;
                }
            }

            foreach (var timedType in timedTypes)
            {
                switch (timedType)
                {
                    case 0:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.TimedMining));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.TimedMining);
                        continue;
                    case 1:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.TimedQuarrying));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.TimedQuarrying);
                        continue;
                    case 2:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.TimedLogging));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.TimedLogging);
                        continue;
                    case 3:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.TimedHarvesting));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.TimedHarvesting);
                        continue;
                }
            }

            foreach (var hiddenType in hiddenTypes)
            {
                switch (hiddenType)
                {
                    case 0:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.HiddenMining));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.HiddenMining);
                        continue;
                    case 1:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.HiddenQuarrying));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.HiddenQuarrying);
                        continue;
                    case 2:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.HiddenLogging));
                        this.AddItemSourceMapLocation(gatheringItem.Item.RowId, mapIds, ItemInfoType.HiddenLogging);
                        continue;
                    case 3:
                        this.AddItemSource(new ItemGatheringSource(gatheringItem, ItemInfoType.HiddenHarvesting));
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

        foreach (var dailySupplyItem in dailySupplyItemSheet)
        {
            for (var index = 0; index < dailySupplyItem.Base.Item.Count; index++)
            {
                var itemId = dailySupplyItem.Base.Item[index];
                var item = itemSheet.GetRowOrDefault(itemId.RowId);
                if (item == null)
                {
                    continue;
                }

                this.AddItemUse(new ItemDailySupplyItemSource(dailySupplyItem, index, item));
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