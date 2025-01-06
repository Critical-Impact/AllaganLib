using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class FishingSpotSheet : ExtendedSheet<FishingSpot, FishingSpotRow, FishingSpotSheet>, IExtendedSheet
{
    private ItemSheet? itemSheet;
    private Dictionary<uint,List<FishingSpotRow>> fishingSpotsByItem;

    public FishingSpotSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public Dictionary<uint, List<FishingSpotRow>> FishingSpotsByItem => this.fishingSpotsByItem;

    public List<FishingSpotRow> GetFishingSpots(uint itemId)
    {
        return this.FishingSpotsByItem.GetValueOrDefault(itemId)?.Where(c => c.TerritoryType.RowId != 0).ToList() ?? new List<FishingSpotRow>();
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public override void CalculateLookups()
    {
        this.fishingSpotsByItem = this.SheetIndexer.OneToMany<FishingSpot, FishingSpotRow, FishingSpotSheet, Item, ItemRow, ItemSheet>(
            this,
            row => row.Items);
    }
}