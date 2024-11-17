using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using Lumina;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets;

public class ItemSheet : ExtendedSheet<Item, ItemRow, ItemSheet>, IExtendedSheet
{
    private readonly List<ItemPatch> itemPatchesList;
    private Dictionary<uint, List<GatheringItemRow>>? gatheringItems;
    private Dictionary<uint, List<FishingSpotRow>>? fishingSpots;
    private GatheringItemSheet? gatheringItemSheet;
    private FishingSpotSheet? fishingSpotSheet;
    private AddonSheet? addonSheet;
    private EquipSlotCategorySheet? equipSlotCategorySheet;
    private EquipRaceCategorySheet? equipRaceCategorySheet;
    private ClassJobCategorySheet? classJobCategorySheet;
    public Dictionary<string, uint>? itemsByName;
    public Dictionary<string, uint>? itemsBySearchString;
    private Dictionary<uint, CabinetCategoryRow>? cabinetCategories;
    private Dictionary<uint, decimal>? itemPatches;
    private Dictionary<uint, string>? itemsSearchStringsById;

    public ItemSheet(
        GameData gameData,
        SheetManager sheetManager,
        SheetIndexer sheetIndexer,
        ItemInfoCache itemInfoCache,
        List<ItemPatch> itemPatches)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.itemPatchesList = itemPatches;
    }

    private decimal currentPatch = new decimal(7.05);

    public decimal GetItemPatch(uint itemId)
    {
        if (this.itemPatches == null)
        {
            this.itemPatches = ItemPatch.ToItemLookup(this.itemPatchesList);
        }

        return this.itemPatches.GetValueOrDefault(itemId, this.currentPatch);
    }

    public Dictionary<string, uint> ItemsByName
    {
        get
        {
            if (this.itemsByName == null)
            {
                this.itemsByName = this.DistinctBy(c => c.Base.Name.ExtractText()).ToDictionary(c => c.Base.Name.ExtractText(), c => c.RowId);
            }

            return this.itemsByName;
        }
    }

    public Dictionary<string, uint> ItemsBySearchString
    {
        get
        {
            if (this.itemsBySearchString == null)
            {
                this.itemsBySearchString = this.DistinctBy(c => c.Base.Name.ExtractText().ToParseable()).ToDictionary(c => c.Base.Name.ExtractText().ToParseable(), c => c.RowId);
            }

            return this.itemsBySearchString;
        }
    }

    public Dictionary<uint, string> ItemsSearchStringsById
    {
        get
        {
            if (this.itemsSearchStringsById == null)
            {
                this.itemsSearchStringsById = this.ToDictionary(c => c.RowId, c => c.Base.Name.ExtractText().ToParseable());
            }

            return this.itemsSearchStringsById;
        }
    }

    public List<ItemSource> GetItemSources(uint itemId)
    {
        return this.ItemInfoCache.GetItemSources(itemId) ?? [];
    }

    public List<ItemSource> GetItemUses(uint itemId)
    {
        return this.ItemInfoCache.GetItemUses(itemId) ?? [];
    }

    public GatheringItemSheet GetGatheringItemSheet()
    {
        return this.gatheringItemSheet ??= this.SheetManager.GetSheet<GatheringItemSheet>();
    }

    public FishingSpotSheet GetFishingSpotSheet()
    {
        return this.fishingSpotSheet ??= this.SheetManager.GetSheet<FishingSpotSheet>();
    }

    public EquipSlotCategorySheet GetEquipSlotCategorySheet()
    {
        return this.equipSlotCategorySheet ??= this.SheetManager.GetSheet<EquipSlotCategorySheet>();
    }

    public ClassJobCategorySheet GetClassJobCategorySheet()
    {
        return this.classJobCategorySheet ??= this.SheetManager.GetSheet<ClassJobCategorySheet>();
    }

    public EquipRaceCategorySheet GetEquipRaceCategorySheet()
    {
        return this.equipRaceCategorySheet ??= this.SheetManager.GetSheet<EquipRaceCategorySheet>();
    }


    public AddonSheet GetAddonSheet()
    {
        return this.addonSheet ??= this.SheetManager.GetSheet<AddonSheet>();
    }

    public List<GatheringItemRow>? GetGatheringItems(uint itemId)
    {
        var gatheringItemSheet = this.GetGatheringItemSheet();

        this.gatheringItems ??= this.SheetIndexer.OneToMany<GatheringItem, GatheringItemRow, GatheringItemSheet, Item, ItemRow, ItemSheet>(
            gatheringItemSheet,
            item => item.Item);

        return this.gatheringItems.GetValueOrDefault(itemId);
    }

    public List<FishingSpotRow>? GetFishingSpots(uint itemId)
    {
        var fishingSpotSheet = this.GetFishingSpotSheet();

        this.fishingSpots ??= this.SheetIndexer.OneToMany<FishingSpot, FishingSpotRow, FishingSpotSheet, Item, ItemRow, ItemSheet>(
            fishingSpotSheet,
            item => item.Items);

        return this.fishingSpots.GetValueOrDefault(itemId);
    }

    public CabinetCategoryRow? GetCabinetCategory(uint itemId)
    {
        if (this.cabinetCategories == null)
        {
            var cabinetSheet = this.SheetManager.GetSheet<CabinetSheet>();
            var cabinetCategorySheet = this.SheetManager.GetSheet<CabinetCategorySheet>();
            this.cabinetCategories = this.SheetIndexer.OneToOne(
                cabinetSheet,
                cabinetCategorySheet,
                row => (row.Base.Item.RowId, row.CabinetCategory));
        }

        return this.cabinetCategories.GetValueOrDefault(itemId);
    }

    public override void CalculateLookups()
    {
    }
}