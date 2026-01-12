#pragma warning disable PendingExcelSchema
using System;
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
using ItemSupplementSource = LuminaSupplemental.Excel.Model.ItemSupplementSource;

using Quest = Lumina.Excel.Sheets.Experimental.Quest;

namespace AllaganLib.GameSheets.Caches;

public class ItemInfoCache
{
    private readonly SheetManager sheetManager;
    private readonly SheetIndexer sheetIndexer;
    private readonly NpcShopCache npcShopCache;
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
    private readonly List<GardeningCrossbreed> gardeningCrossbreeds;
    private readonly List<FieldOpCoffer> fieldOpCoffers;

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
    private readonly Dictionary<uint, List<QuestRequiredItem>> questRequiredItems;

    public ItemInfoCache(
        SheetManager sheetManager,
        SheetIndexer sheetIndexer,
        NpcShopCache npcShopCache,
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
        List<DungeonBossDrop> dungeonBossDrop,
        List<GardeningCrossbreed> gardeningCrossbreeds,
        List<QuestRequiredItem> questRequiredItems,
        List<FieldOpCoffer> fieldOpCoffers)
    {
        this.sheetManager = sheetManager;
        this.sheetIndexer = sheetIndexer;
        this.npcShopCache = npcShopCache;
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
        this.gardeningCrossbreeds = gardeningCrossbreeds;
        this.fieldOpCoffers = fieldOpCoffers;
        this.itemSources = new Dictionary<uint, List<ItemSource>>();
        this.itemUses = new Dictionary<uint, List<ItemSource>>();
        this.itemSourceUseMap = new Dictionary<(uint, uint), List<ItemSource>>();
        this.eNpcIdToIShopLookup = new Dictionary<uint, List<IShop>>();
        this.itemSourceMapLocations = new();
        this.itemUseMapLocations = new();
        this.questRequiredItems = questRequiredItems.GroupBy(c => c.QuestId).ToDictionary(c => c.Key, c => c.ToList());
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
        var housingFurnitureSheet = this.gameData.GetExcelSheet<HousingFurniture>()!;
        var furnitureCatalogSheet = this.gameData.GetExcelSheet<FurnitureCatalogItemList>()!;
        var houseYardObjectSheet = this.gameData.GetExcelSheet<HousingYardObject>()!;
        var housingPresetSheet = this.gameData.GetExcelSheet<HousingPreset>()!;
        var mirageStoreSetItemSheet = this.gameData.GetExcelSheet<MirageStoreSetItem>()!;
        var companyCraftDraftSheet = this.gameData.GetExcelSheet<CompanyCraftDraft>()!;
        var questSheet = this.gameData.GetExcelSheet<Quest>()!;
        var leveSheet = this.gameData.GetExcelSheet<Leve>()!;
        var tripleTriadSheet = this.sheetManager.GetSheet<TripleTriadSheet>()!;
        var enpcBaseSheet = this.sheetManager.GetSheet<ENpcBaseSheet>();
        var pvpSeriesSheet = this.gameData.GetExcelSheet<PvPSeries>()!;
        var collectablesShopSheet = this.sheetManager.GetSheet<CollectablesShopSheet>();
        var zodiacItemSheet = this.gameData.GetExcelSheet<RelicItem>()!;
        var animaItemSheet = this.gameData.GetExcelSheet<AnimaWeaponItem>()!;
        var classJobSheet = this.sheetManager.GetSheet<ClassJobSheet>()!;
        var animaTradeItemSheet = this.sheetManager.GetSheet<AnimaWeapon5TradeItemSheet>()!;


        // var wksMissionUnitSheet = this.gameData.GetExcelSheet<WKSMissionUnit>()!;
        // var wksMissionTodoSheet = this.gameData.GetExcelSheet<WKSMissionToDo>()!;
        // var wksMissionSupplyItemSheet = this.gameData.GetExcelSheet<WKSMissionSupplyItem>()!;
        // var wKSMissionToDoEvalutionItemSheet = this.gameData.GetSubrowExcelSheet<WKSMissionToDoEvalutionItem>()!;
        // var wksItemInfoSheet = this.gameData.GetExcelSheet<WKSItemInfo>()!;
        //
        // foreach (var missionUnit in wksMissionUnitSheet)
        // {
        //     if (missionUnit.RowId == 0)
        //     {
        //         continue;
        //     }
        //
        //     var supplyItem = wksMissionSupplyItemSheet.GetRowOrDefault(missionUnit.WKSMissionSupplyItem);
        //     if (supplyItem != null)
        //     {
        //         var items = new List<(ushort, ushort)>()
        //         {
        //             (todoRow.Value.Unknown3, todoRow.Value.Unknown6),
        //             (todoRow.Value.Unknown4, todoRow.Value.Unknown7),
        //             (todoRow.Value.Unknown5, todoRow.Value.Unknown8),
        //         };
        //         foreach (var item in items)
        //         {
        //             var evaluationItem = item.Item1;
        //             var evaluationQty = item.Item2;
        //             if (evaluationItem != 0)
        //             {
        //                 var itemInfo = wksItemInfoSheet.GetRowOrDefault(evaluationItem);
        //                 if (itemInfo != null && itemInfo.Value.Unknown0 != 0)
        //                 {
        //                     var actualItem = itemSheet.GetRowOrDefault(itemInfo.Value.Unknown0);
        //                     if (actualItem != null)
        //                     {
        //                         var source = new ItemStellarMissionSource(
        //                             actualItem,
        //                             evaluationQty,
        //                             new RowRef<WKSMissionUnit>(this.gameData.Excel, missionUnit.RowId),
        //                             new RowRef<WKSMissionToDo>(this.gameData.Excel, todoRow.Value.RowId),
        //                             new RowRef<WKSItemInfo>(this.gameData.Excel, itemInfo.Value.RowId));
        //                         this.AddItemUse(source);
        //                     }
        //                 }
        //             }
        //         }
        //     }
        // }

        var classJobMap = classJobSheet.Where(c => c.ClassJobType != ClassJobType.Unknown).ToDictionary(c => c.ClassJobType, c => c);

        var zodiacWeapons = new Dictionary<ClassJobType, List<ItemRow>>()
        {
            { ClassJobType.PLD, new List<ItemRow>() },
            { ClassJobType.MNK, new List<ItemRow>() },
            { ClassJobType.WAR, new List<ItemRow>() },
            { ClassJobType.DRG, new List<ItemRow>() },
            { ClassJobType.BRD, new List<ItemRow>() },
            { ClassJobType.WHM, new List<ItemRow>() },
            { ClassJobType.BLM, new List<ItemRow>() },
            { ClassJobType.SMN, new List<ItemRow>() },
            { ClassJobType.SCH, new List<ItemRow>() },
            { ClassJobType.NIN, new List<ItemRow>() },
        };

        foreach (var zodiacItem in zodiacItemSheet)
        {
            zodiacWeapons[ClassJobType.PLD].Add(itemSheet.GetRowOrDefault(zodiacItem.GladiatorItem.RowId)!);
            zodiacWeapons[ClassJobType.PLD].Add(itemSheet.GetRowOrDefault(zodiacItem.ShieldItem.RowId)!);
            zodiacWeapons[ClassJobType.MNK].Add(itemSheet.GetRowOrDefault(zodiacItem.PugilistItem.RowId)!);
            zodiacWeapons[ClassJobType.WAR].Add(itemSheet.GetRowOrDefault(zodiacItem.MarauderItem.RowId)!);
            zodiacWeapons[ClassJobType.DRG].Add(itemSheet.GetRowOrDefault(zodiacItem.LancerItem.RowId)!);
            zodiacWeapons[ClassJobType.BRD].Add(itemSheet.GetRowOrDefault(zodiacItem.ArcherItem.RowId)!);
            zodiacWeapons[ClassJobType.WHM].Add(itemSheet.GetRowOrDefault(zodiacItem.ConjurerItem.RowId)!);
            zodiacWeapons[ClassJobType.BLM].Add(itemSheet.GetRowOrDefault(zodiacItem.ThaumaturgeItem.RowId)!);
            zodiacWeapons[ClassJobType.SMN].Add(itemSheet.GetRowOrDefault(zodiacItem.ArcanistSMNItem.RowId)!);
            zodiacWeapons[ClassJobType.SCH].Add(itemSheet.GetRowOrDefault(zodiacItem.ArcanistSCHItem.RowId)!);
            zodiacWeapons[ClassJobType.NIN].Add(itemSheet.GetRowOrDefault(zodiacItem.RogueItem.RowId)!);
        }

        foreach (var zodiacWeaponSet in zodiacWeapons)
        {
            foreach (var item in zodiacWeaponSet.Value)
            {
                this.AddItemUse(new ItemZodiacWeaponSource(item, classJobMap[zodiacWeaponSet.Key], zodiacWeaponSet.Value));
            }
        }

        var animaWeapons = new Dictionary<ClassJobType, List<ItemRow>>()
        {
            { ClassJobType.PLD, new List<ItemRow>() },
            { ClassJobType.MNK, new List<ItemRow>() },
            { ClassJobType.WAR, new List<ItemRow>() },
            { ClassJobType.DRG, new List<ItemRow>() },
            { ClassJobType.BRD, new List<ItemRow>() },
            { ClassJobType.WHM, new List<ItemRow>() },
            { ClassJobType.BLM, new List<ItemRow>() },
            { ClassJobType.SMN, new List<ItemRow>() },
            { ClassJobType.SCH, new List<ItemRow>() },
            { ClassJobType.NIN, new List<ItemRow>() },
            { ClassJobType.AST, new List<ItemRow>() },
            { ClassJobType.MCH, new List<ItemRow>() },
            { ClassJobType.DRK, new List<ItemRow>() },
        };

        foreach (var animaItem in animaItemSheet)
        {
            animaWeapons[ClassJobType.PLD].Add(itemSheet.GetRowOrDefault(animaItem.Item[0].RowId)!);
            animaWeapons[ClassJobType.MNK].Add(itemSheet.GetRowOrDefault(animaItem.Item[1].RowId)!);
            animaWeapons[ClassJobType.WAR].Add(itemSheet.GetRowOrDefault(animaItem.Item[2].RowId)!);
            animaWeapons[ClassJobType.DRG].Add(itemSheet.GetRowOrDefault(animaItem.Item[3].RowId)!);
            animaWeapons[ClassJobType.BRD].Add(itemSheet.GetRowOrDefault(animaItem.Item[4].RowId)!);
            animaWeapons[ClassJobType.WHM].Add(itemSheet.GetRowOrDefault(animaItem.Item[5].RowId)!);
            animaWeapons[ClassJobType.BLM].Add(itemSheet.GetRowOrDefault(animaItem.Item[6].RowId)!);
            animaWeapons[ClassJobType.SMN].Add(itemSheet.GetRowOrDefault(animaItem.Item[7].RowId)!);
            animaWeapons[ClassJobType.SCH].Add(itemSheet.GetRowOrDefault(animaItem.Item[8].RowId)!);
            animaWeapons[ClassJobType.NIN].Add(itemSheet.GetRowOrDefault(animaItem.Item[9].RowId)!);
            animaWeapons[ClassJobType.MCH].Add(itemSheet.GetRowOrDefault(animaItem.Item[10].RowId)!);
            animaWeapons[ClassJobType.DRK].Add(itemSheet.GetRowOrDefault(animaItem.Item[11].RowId)!);
            animaWeapons[ClassJobType.AST].Add(itemSheet.GetRowOrDefault(animaItem.Item[12].RowId)!);
            animaWeapons[ClassJobType.PLD].Add(itemSheet.GetRowOrDefault(animaItem.Item[13].RowId)!);
        }

        foreach (var animaWeaponSet in animaWeapons)
        {
            foreach (var item in animaWeaponSet.Value)
            {
                this.AddItemUse(new ItemAnimaWeaponSource(item, classJobMap[animaWeaponSet.Key], animaWeaponSet.Value));
            }
        }

        foreach (var seriesReward in pvpSeriesSheet)
        {
            for (var levelIndex = 0; levelIndex < seriesReward.LevelRewards.Count; levelIndex++)
            {
                var reward = seriesReward.LevelRewards[levelIndex];
                for (var index = 0; index < reward.LevelRewardItem.Count; index++)
                {
                    var rewardItem = reward.LevelRewardItem[index];
                    if (rewardItem.RowId == 0)
                    {
                        continue;
                    }

                    var itemRow = itemSheet.GetRowOrDefault(rewardItem.RowId);
                    if (itemRow != null)
                    {
                        var source = new ItemPVPSeriesSource(
                            itemRow,
                            new RowRef<PvPSeries>(this.gameData.Excel, seriesReward.RowId),
                            levelIndex,
                            index);
                        this.AddItemSource(source);
                    }
                }
            }
        }

        foreach (var tripleTriad in tripleTriadSheet)
        {
            foreach (var reward in tripleTriad.Base.ItemPossibleReward)
            {
                if (reward.RowId == 0 || !reward.IsValid)
                {
                    continue;
                }
                var itemRow = itemSheet.GetRowOrDefault(reward.RowId);
                if (itemRow != null)
                {
                    var source = new ItemTripleTriadSource(tripleTriad, itemRow);
                    this.AddItemSource(source);
                }
            }
        }

        var classesWithRelicQuests = classJobSheet.Where(c => c.Base.RelicQuest.RowId != 0).ToDictionary(c => c.Base.RelicQuest.RowId, c => c);

        foreach (var quest in questSheet)
        {
            if (this.questRequiredItems.TryGetValue(quest.RowId, out var requiredItems))
            {
                foreach (var requiredItem in requiredItems)
                {
                    var itemId = requiredItem.ItemId;
                    if (itemId != 0)
                    {
                        var itemRow = itemSheet.GetRowOrDefault(itemId);
                        if (itemRow != null)
                        {
                            var source = new ItemQuestUse(
                                itemRow,
                                requiredItems,
                                new RowRef<Quest>(this.gameData.Excel, quest.RowId));
                            this.AddItemUse(source);
                        }
                    }
                }

                if (classesWithRelicQuests.ContainsKey(quest.RowId))
                {
                    var relatedClass = classesWithRelicQuests[quest.RowId];

                    foreach (var relicItem in zodiacWeapons[relatedClass.ClassJobType])
                    {
                        var source = new ItemQuestSource(
                            relicItem,
                            requiredItems,
                            new RowRef<Quest>(this.gameData.Excel, quest.RowId));
                        this.AddItemSource(source);
                    }
                }
            }

            requiredItems ??= [];

            foreach (var reward in quest.Reward)
            {
                if (reward.Is<Item>())
                {
                    if (reward.TryGetValue(out Item rewardItem))
                    {
                        var itemRow = itemSheet.GetRowOrDefault(rewardItem.RowId);
                        if (itemRow != null)
                        {
                            var source = new ItemQuestSource(itemRow, requiredItems, new RowRef<Quest>(this.gameData.Excel, quest.RowId));
                            this.AddItemSource(source);
                        }
                    }
                }

                if (reward.IsSubrow<QuestClassJobReward>())
                {
                    if (reward.TryGetValueSubrow<QuestClassJobReward>(out var rewardJob))
                    {
                        for (var index = 0; index < rewardJob.Count; index++)
                        {
                            var rewardJobSubrow = rewardJob[index];
                            for (var i = 0; i < rewardJobSubrow.RewardItem.Count; i++)
                            {
                                var rewardSubrowItem = rewardJobSubrow.RewardItem[i];
                                if (rewardSubrowItem.RowId == 0)
                                {
                                    continue;
                                }
                                var itemRow = itemSheet.GetRowOrDefault(rewardSubrowItem.RowId);
                                if (itemRow != null)
                                {
                                    var source = new ItemQuestSource(itemRow, requiredItems, new RowRef<Quest>(this.gameData.Excel, quest.RowId), new SubrowRef<QuestClassJobReward>(this.gameData.Excel, rewardJobSubrow.RowId), index);
                                    this.AddItemSource(source);
                                }
                            }
                            for (var i = 0; i < rewardJobSubrow.RequiredItem.Count; i++)
                            {
                                var requiredItemSubrow = rewardJobSubrow.RequiredItem[i];
                                if (requiredItemSubrow.RowId == 0)
                                {
                                    continue;
                                }
                                var itemRow = itemSheet.GetRowOrDefault(requiredItemSubrow.RowId);
                                if (itemRow != null)
                                {
                                    var source = new ItemQuestUse(itemRow, requiredItems, new RowRef<Quest>(this.gameData.Excel, quest.RowId), new SubrowRef<QuestClassJobReward>(this.gameData.Excel, rewardJobSubrow.RowId), index);
                                    this.AddItemUse(source);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var catalyst in quest.ItemCatalyst)
            {
                if (catalyst.RowId == 0)
                {
                    continue;
                }
                var itemRow = itemSheet.GetRowOrDefault(catalyst.RowId);
                if (itemRow != null)
                {
                    var source = new ItemQuestSource(itemRow, requiredItems, new RowRef<Quest>(this.gameData.Excel, quest.RowId));
                    this.AddItemSource(source);
                }
            }

            foreach (var optionalReward in quest.OptionalItemReward)
            {
                if (optionalReward.RowId == 0)
                {
                    continue;
                }
                var itemRow = itemSheet.GetRowOrDefault(optionalReward.RowId);
                if (itemRow != null)
                {
                    var source = new ItemQuestSource(itemRow, requiredItems, new RowRef<Quest>(this.gameData.Excel, quest.RowId));
                    this.AddItemSource(source);
                }
            }

            if (quest.QuestClassJobSupply.RowId != 0)
            {
                for (var index = 0; index < quest.QuestClassJobSupply.Value.Count; index++)
                {
                    var questClassJobSupplyRef = quest.QuestClassJobSupply.Value[index];
                    if (questClassJobSupplyRef.Item.RowId != 0)
                    {
                        var itemRow = itemSheet.GetRowOrDefault(questClassJobSupplyRef.Item.RowId);
                        if (itemRow != null)
                        {
                            var source = new ItemQuestUse(
                                itemRow,
                                requiredItems,
                                new RowRef<Quest>(this.gameData.Excel, quest.RowId),
                                new SubrowRef<QuestClassJobSupply>(this.gameData.Excel, questClassJobSupplyRef.RowId)
                                ,index);
                            this.AddItemUse(source);
                        }
                    }
                }
            }
        }

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

        foreach (var gardeningCrossbreed in this.gardeningCrossbreeds)
        {
            var resultItem = itemSheet.GetRow(gardeningCrossbreed.ItemResultId);
            var requirement1 = itemSheet.GetRow(gardeningCrossbreed.ItemRequirement1Id);
            var requirement2 = itemSheet.GetRow(gardeningCrossbreed.ItemRequirement2Id);


            var source = new ItemGardeningCrossbreedSource(resultItem, resultItem, requirement1, requirement2);
            this.AddItemSource(source);

            source = new ItemGardeningCrossbreedSource(requirement1, resultItem, requirement1, requirement2);
            this.AddItemUse(source);

            source = new ItemGardeningCrossbreedSource(requirement2, resultItem, requirement1, requirement2);
            this.AddItemUse(source);
        }

        foreach (var fieldOpCoffer in this.fieldOpCoffers)
        {
            var resultItem = itemSheet.GetRowOrDefault(fieldOpCoffer.ItemId);

            if (resultItem != null)
            {
                switch (fieldOpCoffer.Type)
                {
                    case FieldOpType.Pagos:
                        this.AddItemSource(new ItemPagosTreasureCofferSource(resultItem, fieldOpCoffer));
                        break;
                    case FieldOpType.Pyros:
                        this.AddItemSource(new ItemPyrosTreasureCofferSource(resultItem, fieldOpCoffer));
                        break;
                    case FieldOpType.Hydatos:
                        this.AddItemSource(new ItemHydatosTreasureCofferSource(resultItem, fieldOpCoffer));
                        break;
                    case FieldOpType.OccultTreasure:
                        this.AddItemSource(new ItemOccultTreasureCofferSource(resultItem, fieldOpCoffer));
                        break;
                    case FieldOpType.OccultPot:
                        this.AddItemSource(new ItemOccultPotSource(resultItem, fieldOpCoffer));
                        break;
                    case FieldOpType.OccultGoldenCoffer:
                        this.AddItemSource(new ItemOccultGoldenCofferSource(resultItem, fieldOpCoffer));
                        break;
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

        var furnitureCatalogByItemId = furnitureCatalogSheet.Where(c => c.Item.RowId != 0).DistinctBy(c => c.Item.RowId).ToDictionary(c => c.Item.RowId, c => c.RowId);

        foreach (var housingFurniture in housingFurnitureSheet)
        {
            if (!housingFurniture.Item.IsValid || housingFurniture.Item.RowId == 0)
            {
                continue;
            }

            var item = itemSheet.GetRowOrDefault(housingFurniture.Item.RowId);

            if (item != null)
            {
                var catalogItem = new RowRef<FurnitureCatalogItemList>(
                    this.gameData.Excel,
                    furnitureCatalogByItemId.TryGetValue(item.RowId, out var rowId) ? rowId : 0);

                var source = new ItemFurnitureSource(item, new RowRef<HousingFurniture>(this.gameData.Excel, housingFurniture.RowId), catalogItem);
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
            if (mirageStoreSetItem.RowId == 0)
            {
                // SE decided to put in a row 0 with an item with the ID = 1
                continue;
            }
            List<uint> ids = new List<uint>
                {
                    mirageStoreSetItem.MainHand.RowId, mirageStoreSetItem.OffHand.RowId, mirageStoreSetItem.Head.RowId,
                    mirageStoreSetItem.Body.RowId, mirageStoreSetItem.Hands.RowId, mirageStoreSetItem.Legs.RowId,
                    mirageStoreSetItem.Feet.RowId, mirageStoreSetItem.Earrings.RowId,
                    mirageStoreSetItem.Necklace.RowId, mirageStoreSetItem.Bracelets.RowId, mirageStoreSetItem.Ring.RowId,
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
            if (cabinet.Base.Item.RowId == 0)
            {
                continue;
            }
            var item = itemSheet.GetRowOrDefault(cabinet.Base.Item.RowId);

            if (item != null)
            {
                var use = new ItemArmoireSource(cabinet, item);
                this.AddItemUse(use);
            }
        }

        foreach (var leve in leveSheet)
        {
            if (leve.DataId.Is<CraftLeve>())
            {
                if (leve.DataId.TryGetValue(out CraftLeve craftLeve))
                {
                    for (var index = 0; index < craftLeve.Item.Count; index++)
                    {
                        if (craftLeve.Item[index].RowId == 0)
                        {
                            continue;
                        }
                        var item = itemSheet.GetRowOrDefault(craftLeve.Item[index].RowId);

                        if (item != null)
                        {
                            var use = new ItemCraftLeveUse(new RowRef<CraftLeve>(this.gameData.Excel, craftLeve.RowId), new RowRef<Leve>(this.gameData.Excel, leve.RowId), item, index);
                            this.AddItemUse(use);
                        }
                    }
                    var rewardItem = leve.LeveRewardItem.ValueNullable;
                    if (rewardItem != null)
                    {
                        var rewardGroup = rewardItem.Value.LeveRewardItemGroup;
                        for (var rewardItemIndex = 0; rewardItemIndex < rewardGroup.Count; rewardItemIndex++)
                        {
                            var c = rewardGroup[rewardItemIndex];
                            if (c.ValueNullable != null)
                            {
                                var reward = c.Value;
                                for (var groupIndex = 0; groupIndex < reward.Item.Count; groupIndex++)
                                {
                                    var item = reward.Item[groupIndex];
                                    var itemRow = itemSheet.GetRowOrDefault(item.RowId);
                                    if (itemRow != null)
                                    {
                                        var source = new ItemCraftLeveSource(
                                            new RowRef<CraftLeve>(this.gameData.Excel, craftLeve.RowId),
                                            new RowRef<Leve>(this.gameData.Excel, leve.RowId),
                                            new RowRef<LeveRewardItem>(this.gameData.Excel, rewardItem.Value.RowId),
                                            rewardItemIndex,
                                            new RowRef<LeveRewardItemGroup>(this.gameData.Excel, reward.RowId),
                                            groupIndex,
                                            itemRow);
                                        this.AddItemSource(source);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (leve.DataId.Is<GatheringLeve>())
            {
                if (leve.DataId.TryGetValue(out GatheringLeve gatheringLeve))
                {
                    var rewardItem = leve.LeveRewardItem.ValueNullable;
                    if (rewardItem != null)
                    {
                        var rewardGroup = rewardItem.Value.LeveRewardItemGroup;
                        for (var rewardItemIndex = 0; rewardItemIndex < rewardGroup.Count; rewardItemIndex++)
                        {
                            var c = rewardGroup[rewardItemIndex];
                            if (c.ValueNullable != null)
                            {
                                var reward = c.Value;
                                for (var groupIndex = 0; groupIndex < reward.Item.Count; groupIndex++)
                                {
                                    var item = reward.Item[groupIndex];
                                    var itemRow = itemSheet.GetRowOrDefault(item.RowId);
                                    if (itemRow != null)
                                    {
                                        var source = new ItemGatheringLeveSource(
                                            new RowRef<GatheringLeve>(this.gameData.Excel, gatheringLeve.RowId),
                                            new RowRef<Leve>(this.gameData.Excel, leve.RowId),
                                            new RowRef<LeveRewardItem>(this.gameData.Excel, rewardItem.Value.RowId),
                                            rewardItemIndex,
                                            new RowRef<LeveRewardItemGroup>(this.gameData.Excel, reward.RowId),
                                            groupIndex,
                                            itemRow);
                                        this.AddItemSource(source);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (leve.DataId.Is<CompanyLeve>())
            {
                if (leve.DataId.TryGetValue(out CompanyLeve companyLeve))
                {
                    var stormSeal = itemSheet.GetRowOrDefault(20)!;
                    var serpentSeal = itemSheet.GetRowOrDefault(21)!;
                    var flameSeal = itemSheet.GetRowOrDefault(22)!;

                    var source = new ItemCompanyLeveSource(new RowRef<CompanyLeve>(this.gameData.Excel, companyLeve.RowId), new RowRef<Leve>(this.gameData.Excel, leve.RowId), stormSeal);
                    this.AddItemSource(source);

                    source = new ItemCompanyLeveSource(new RowRef<CompanyLeve>(this.gameData.Excel, companyLeve.RowId), new RowRef<Leve>(this.gameData.Excel, leve.RowId), serpentSeal);
                    this.AddItemSource(source);

                    source = new ItemCompanyLeveSource(new RowRef<CompanyLeve>(this.gameData.Excel, companyLeve.RowId), new RowRef<Leve>(this.gameData.Excel, leve.RowId), flameSeal);
                    this.AddItemSource(source);
                }
            }
            else if (leve.DataId.Is<BattleLeve>())
            {
                if (leve.DataId.TryGetValue(out BattleLeve battleLeve))
                {
                    var rewardItem = leve.LeveRewardItem.ValueNullable;
                    if (rewardItem != null)
                    {
                        var rewardGroup = rewardItem.Value.LeveRewardItemGroup;
                        for (var rewardItemIndex = 0; rewardItemIndex < rewardGroup.Count; rewardItemIndex++)
                        {
                            var c = rewardGroup[rewardItemIndex];
                            if (c.ValueNullable != null)
                            {
                                var reward = c.Value;
                                for (var groupIndex = 0; groupIndex < reward.Item.Count; groupIndex++)
                                {
                                    var item = reward.Item[groupIndex];
                                    var itemRow = itemSheet.GetRowOrDefault(item.RowId);
                                    if (itemRow != null)
                                    {
                                        var source = new ItemBattleLeveSource(
                                            new RowRef<BattleLeve>(this.gameData.Excel, battleLeve.RowId),
                                            new RowRef<Leve>(this.gameData.Excel, leve.RowId),
                                            new RowRef<LeveRewardItem>(this.gameData.Excel, rewardItem.Value.RowId),
                                            rewardItemIndex,
                                            new RowRef<LeveRewardItemGroup>(this.gameData.Excel, reward.RowId),
                                            groupIndex,
                                            itemRow);
                                        this.AddItemSource(source);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach (var recipe in recipeSheet)
        {
            if (recipe.Base.ItemResult.RowId == 0)
            {
                continue;
            }
            var result = itemSheet.GetRow(recipe.Base.ItemResult.RowId);
            var source = new ItemCraftResultSource(result, recipe);
            this.AddItemSource(source);
            foreach (var ingredientCount in recipe.IngredientCounts)
            {
                if (ingredientCount.Key == 0)
                {
                    continue;
                }
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

        foreach (var collectableShop in collectablesShopSheet)
        {
            var mapIds = collectableShop.MapIds;

            foreach (var npc in collectableShop.ENpcs)
            {
                this.AddShopNpcLookup(collectableShop, npc.RowId);
            }

            foreach (var collectableShopListing in collectableShop.CollectablesShopListings)
            {
                var source = new ItemCollectablesShopSource(collectableShopListing, collectableShop);
                this.AddItemSource(source);
                this.AddItemUse(source);
                this.AddItemSourceUseCombo(source, source);

                this.AddItemSourceMapLocation(collectableShopListing.Reward.Item.RowId, mapIds, ItemInfoType.CollectablesShop);
                this.AddItemUseMapLocation(collectableShopListing.Costs.First().Item.RowId, mapIds, ItemInfoType.CollectablesShop);
            }
        }

        foreach (var animaShop in animaTradeItemSheet)
        {
            var mapIds = animaShop.MapIds;

            foreach (var npc in animaShop.ENpcs)
            {
                this.AddShopNpcLookup(animaShop, npc.RowId);
            }

            foreach (var animaShopListing in animaShop.AnimaWeapon5TradeItemListings)
            {
                // Fix for blank items that SQ seems to have added
                if (animaShopListing.Rewards.All(c => c.Item.Icon == 0))
                {
                    continue;
                }

                List<ItemSource> sources = new();
                foreach (var shopListingItem in animaShopListing.Rewards)
                {
                    var source = new ItemAnimaShopSource(shopListingItem.Item, null, shopListingItem, animaShopListing, animaShop);
                    this.AddItemSource(source);
                    this.AddItemSourceMapLocation(shopListingItem.Item.RowId, mapIds, ItemInfoType.SpecialShop);
                    sources.Add(source);

                    foreach (var cost in animaShopListing.Costs)
                    {
                        var use = new ItemAnimaShopSource(cost.Item, cost.Item, shopListingItem, animaShopListing, animaShop);
                        this.AddItemUse(use);
                        this.AddItemUseMapLocation(cost.Item.RowId, mapIds, ItemInfoType.SpecialShop);
                        this.AddItemSourceUseCombo(source, use);
                    }
                }
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
                            var use = new ItemSpecialShopSource(cost.Item, cost.Item, shopListingItem, specialShopListing, specialShop);
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

            ItemSources.ItemSupplementSource source;
            switch (supplementalItem.ItemSupplementSource)
            {
                case ItemSupplementSource.Desynth:
                    source = new ItemDesynthSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Reduction:
                    source = new ItemReductionSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Loot:
                    source = new ItemLootSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Gardening:
                    source = new ItemGardeningSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.CardPacks:
                    source = new ItemCardPackSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Coffer:
                    source = new ItemCofferSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.PalaceOfTheDead:
                    source = new ItemPalaceOfTheDeadSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.HeavenOnHigh:
                    source = new ItemHeavenOnHighSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.EurekaOrthos:
                    source = new ItemEurekaOrthosSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Anemos:
                    source = new ItemAnemosSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Pagos:
                    source = new ItemPagosSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Pyros:
                    source = new ItemPyrosSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Hydatos:
                    source = new ItemHydatosSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Bozja:
                    source = new ItemBozjaSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.Logogram:
                    source = new ItemLogogramSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.PilgrimsTraverse:
                    source = new ItemPilgrimsTraverseSource(item, sourceItem, supplementalItem);
                    break;
                case ItemSupplementSource.SkybuilderHandIn:
                    continue;
                case ItemSupplementSource.Unknown:
                    continue;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.AddItemSource(source);
            this.AddItemUse(source);
            this.AddItemSourceUseCombo(source, source);
        }

        foreach (var satisfactionSupply in satisfactionSupplySheet)
        {
            if (satisfactionSupply.Base.Item.RowId == 0)
            {
                continue;
            }
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
                if (row.ItemTradeIn.RowId == 0)
                {
                    continue;
                }
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
            if (aquariumRow.Base.Item.RowId == 0)
            {
                continue;
            }
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
                    if (rowRef.RowId == 0)
                    {
                        continue;
                    }
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