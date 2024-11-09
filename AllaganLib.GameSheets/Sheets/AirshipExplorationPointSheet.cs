using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets;

public class AirshipExplorationPointSheet : ExtendedSheet<AirshipExplorationPoint, AirshipExplorationPointRow, AirshipExplorationPointSheet>, IExtendedSheet
{
    private readonly List<AirshipDrop> airshipDrops;
    private readonly List<AirshipUnlock> airshipUnlocks;
    private readonly Dictionary<uint, List<uint>> airshipExplorationPointsByItem;
    private readonly Dictionary<uint, List<uint>> itemsByAirshipExplorationPoint;
    private readonly Dictionary<uint, uint> airshipUnlockByPoint;
    private readonly Dictionary<uint, uint> airshipPointByUnlock;
    private ItemSheet? itemSheet;

    public AirshipExplorationPointSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache, List<AirshipDrop> airshipDrops, List<AirshipUnlock> airshipUnlocks)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.airshipDrops = airshipDrops;
        this.airshipUnlocks = airshipUnlocks;

        this.airshipExplorationPointsByItem = new Dictionary<uint, List<uint>>();
        this.itemsByAirshipExplorationPoint = new Dictionary<uint, List<uint>>();
        this.airshipUnlockByPoint = new Dictionary<uint, uint>();
        this.airshipPointByUnlock = new Dictionary<uint, uint>();
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public override void CalculateLookups()
    {
        this.airshipExplorationPointsByItem.Clear();
        this.itemsByAirshipExplorationPoint.Clear();

        foreach (var airshipDrop in this.airshipDrops)
        {
            var airshipExplorationPointId = airshipDrop.AirshipExplorationPointId;
            var itemId = airshipDrop.ItemId;

            if (!this.airshipExplorationPointsByItem.ContainsKey(itemId))
            {
                this.airshipExplorationPointsByItem[itemId] = new List<uint>();
            }

            this.airshipExplorationPointsByItem[itemId].Add(airshipExplorationPointId);

            if (!this.itemsByAirshipExplorationPoint.ContainsKey(airshipExplorationPointId))
            {
                this.itemsByAirshipExplorationPoint[airshipExplorationPointId] = new List<uint>();
            }

            this.itemsByAirshipExplorationPoint[airshipExplorationPointId].Add(itemId);
        }

        foreach (var airshipUnlock in this.airshipUnlocks)
        {
            var airshipExplorationPointId = airshipUnlock.AirshipExplorationPointId;
            var airshipExplorationPointUnlockId = airshipUnlock.AirshipExplorationPointUnlockId;

            this.airshipUnlockByPoint.TryAdd(airshipExplorationPointId, airshipExplorationPointUnlockId);

            this.airshipPointByUnlock.TryAdd(airshipExplorationPointUnlockId, airshipExplorationPointId);
        }
    }

    /// <summary>
    /// Method to get a list of related airship exploration points by item id.
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public List<uint> GetAirshipExplorationPointsByItem(uint itemId)
    {
        if (this.airshipExplorationPointsByItem.TryGetValue(itemId, out var explorationPoints))
        {
            return explorationPoints;
        }
        return new List<uint>(); // Return an empty list if no exploration points are found
    }

    /// <summary>
    /// Method to get a list of related items by airship exploration point id.
    /// </summary>
    /// <param name="airshipExplorationPointId"></param>
    /// <returns></returns>
    public List<uint> GetItemsByAirshipExplorationPoint(uint airshipExplorationPointId)
    {
        if (this.itemsByAirshipExplorationPoint.TryGetValue(airshipExplorationPointId, out var items))
        {
            return items;
        }
        return new List<uint>(); // Return an empty list if no items are found
    }

    /// <summary>
    /// Method to get the unlock ID by airship exploration point ID.
    /// </summary>
    /// <param name="airshipExplorationPointId"></param>
    /// <returns></returns>
    public uint? GetUnlockByPoint(uint airshipExplorationPointId)
    {
        if (this.airshipUnlockByPoint.TryGetValue(airshipExplorationPointId, out var unlockId))
        {
            return unlockId;
        }

        return null;
    }

    /// <summary>
    /// Method to get the airship exploration point ID by unlock ID.
    /// </summary>
    /// <param name="airshipExplorationPointUnlockId"></param>
    /// <returns></returns>
    public uint? GetPointByUnlock(uint airshipExplorationPointUnlockId)
    {
        if (this.airshipPointByUnlock.TryGetValue(airshipExplorationPointUnlockId, out var explorationPointId))
        {
            return explorationPointId;
        }

        return null;
    }

}