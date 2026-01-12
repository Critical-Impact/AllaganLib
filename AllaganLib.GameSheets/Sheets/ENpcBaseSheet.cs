using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Helpers;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Sheets;

public class ENpcBaseSheet : ExtendedSheet<ENpcBase, ENpcBaseRow, ENpcBaseSheet>, IExtendedSheet
{
    private readonly NpcLevelCache levelCache;
    private readonly NpcShopCache shopCache;
    private readonly List<HouseVendor> houseVendors;
    private Dictionary<uint, HouseVendor> houseVendorsByNpcId;
    private ENpcResidentSheet? eNpcResidentSheet;
    private ItemSheet? itemSheet;
    private Dictionary<uint, List<IShop>> shopsByNpcId;
    private Dictionary<uint, List<ItemRow>> itemsByNpcId;

    public ENpcBaseSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache, NpcLevelCache levelCache, NpcShopCache shopCache, List<HouseVendor> houseVendors)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.levelCache = levelCache;
        this.shopCache = shopCache;
        this.houseVendors = houseVendors;
        this.houseVendorsByNpcId = new();
        this.shopsByNpcId = new();
    }

    public ENpcResidentSheet GetENpcResidentSheet()
    {
        return this.eNpcResidentSheet ??= this.SheetManager.GetSheet<ENpcResidentSheet>();
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public bool IsVendor(uint npcId)
    {
        return this.shopCache.GetShops(npcId) != null;
    }

    public bool IsHouseVendor(uint npcId)
    {
        return this.houseVendorsByNpcId.ContainsKey(npcId);
    }

    public bool IsHouseVendorChild(uint npcId)
    {
        return this.houseVendorsByNpcId.TryGetValue(npcId, out var houseVendor) && houseVendor.ParentId != 0;
    }

    public bool IsCalamitySalvager(uint npcId)
    {
        return HardcodedItems.CalamitySalvagers.Contains(npcId);
    }

    public HashSet<NpcLocation>? GetLocations(uint npcId)
    {
        return this.levelCache.GetLocations(npcId);
    }

    public override void CalculateLookups()
    {
        this.houseVendorsByNpcId = this.houseVendors.ToDictionary(c => c.ENpcResidentId, c => c);
    }
}