using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets;

public class SubmarineExplorationSheet : ExtendedSheet<SubmarineExploration, SubmarineExplorationRow, SubmarineExplorationSheet>, IExtendedSheet
{
    private readonly List<SubmarineDrop> submarineDrops;
    private readonly List<SubmarineUnlock> submarineUnlocks;
    private readonly Dictionary<uint, List<uint>> submarineExplorationsByItem;
    private readonly Dictionary<uint, List<uint>> itemsBySubmarineExploration;
    private readonly Dictionary<uint, uint> submarineUnlockByPoint;
    private readonly Dictionary<uint, uint> submarinePointByUnlock;
    private ItemSheet? itemSheet;

    public SubmarineExplorationSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache, List<SubmarineDrop> submarineDrops, List<SubmarineUnlock> submarineUnlocks)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.submarineDrops = submarineDrops;
        this.submarineUnlocks = submarineUnlocks;

        this.submarineExplorationsByItem = new Dictionary<uint, List<uint>>();
        this.itemsBySubmarineExploration = new Dictionary<uint, List<uint>>();
        this.submarineUnlockByPoint = new Dictionary<uint, uint>();
        this.submarinePointByUnlock = new Dictionary<uint, uint>();
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public override void CalculateLookups()
    {
        this.submarineExplorationsByItem.Clear();
        this.itemsBySubmarineExploration.Clear();

        foreach (var submarineDrop in this.submarineDrops)
        {
            var submarineExplorationId = submarineDrop.SubmarineExplorationId;
            var itemId = submarineDrop.ItemId;

            if (!this.submarineExplorationsByItem.ContainsKey(itemId))
            {
                this.submarineExplorationsByItem[itemId] = new List<uint>();
            }

            this.submarineExplorationsByItem[itemId].Add(submarineExplorationId);

            if (!this.itemsBySubmarineExploration.ContainsKey(submarineExplorationId))
            {
                this.itemsBySubmarineExploration[submarineExplorationId] = new List<uint>();
            }

            this.itemsBySubmarineExploration[submarineExplorationId].Add(itemId);
        }

        foreach (var submarineUnlock in this.submarineUnlocks)
        {
            var submarineExplorationId = submarineUnlock.SubmarineExplorationId;
            var submarineExplorationUnlockId = submarineUnlock.SubmarineExplorationUnlockId;

            this.submarineUnlockByPoint.TryAdd(submarineExplorationId, submarineExplorationUnlockId);

            this.submarinePointByUnlock.TryAdd(submarineExplorationUnlockId, submarineExplorationId);
        }
    }

    /// <summary>
    /// Method to get a list of related submarine exploration points by item id.
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public List<uint> GetSubmarineExplorationsByItem(uint itemId)
    {
        if (this.submarineExplorationsByItem.TryGetValue(itemId, out var explorationPoints))
        {
            return explorationPoints;
        }
        return new List<uint>(); // Return an empty list if no exploration points are found
    }

    /// <summary>
    /// Method to get a list of related items by submarine exploration point id.
    /// </summary>
    /// <param name="submarineExplorationId"></param>
    /// <returns></returns>
    public List<uint> GetItemsBySubmarineExploration(uint submarineExplorationId)
    {
        if (this.itemsBySubmarineExploration.TryGetValue(submarineExplorationId, out var items))
        {
            return items;
        }
        return new List<uint>(); // Return an empty list if no items are found
    }

    /// <summary>
    /// Method to get the unlock ID by submarine exploration point ID.
    /// </summary>
    /// <param name="submarineExplorationId"></param>
    /// <returns></returns>
    public uint? GetUnlockByPoint(uint submarineExplorationId)
    {
        if (this.submarineUnlockByPoint.TryGetValue(submarineExplorationId, out var unlockId))
        {
            return unlockId;
        }

        return null;
    }

    /// <summary>
    /// Method to get the submarine exploration point ID by unlock ID.
    /// </summary>
    /// <param name="submarineExplorationUnlockId"></param>
    /// <returns></returns>
    public uint? GetPointByUnlock(uint submarineExplorationUnlockId)
    {
        if (this.submarinePointByUnlock.TryGetValue(submarineExplorationUnlockId, out var explorationPointId))
        {
            return explorationPointId;
        }

        return null;
    }

}