using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina;
using Lumina.Data.Files;
using Lumina.Data.Parsing.Layer;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.Caches;

public class NpcLevelCache
{
    private readonly ExcelSheet<TerritoryType> territoryTypeSheet;
    private readonly ExcelSheet<Map> mapSheet;
    private readonly ExcelSheet<Level> levelSheet;
    private readonly List<ENpcPlace> eNpcPlaces;
    private readonly GameData gameData;
    private Dictionary<uint, Dictionary<uint, RowRef<Map>>> npcLayerCache;
    private Dictionary<(uint, sbyte), uint>? mapIdByTerritoryTypeAndMapIndex;
    private Dictionary<uint,HashSet<NpcLocation>> locations;

    public NpcLevelCache(
        List<ENpcPlace> eNpcPlaces,
        GameData gameData)
    {
        this.gameData = gameData;
        this.territoryTypeSheet = gameData.GetExcelSheet<TerritoryType>()!;
        this.mapSheet = gameData.GetExcelSheet<Map>()!;
        this.levelSheet = gameData.GetExcelSheet<Level>()!;
        this.eNpcPlaces = eNpcPlaces;
        this.npcLayerCache = new Dictionary<uint, Dictionary<uint, RowRef<Map>>>();
        this.locations = new();
    }

    private uint GetMapIdByTerritoryTypeAndMapIndex(uint territoryTypeId, sbyte mapIndex)
    {
        if (this.mapIdByTerritoryTypeAndMapIndex == null)
        {
            this.mapIdByTerritoryTypeAndMapIndex = new Dictionary<(uint, sbyte), uint>();
            foreach (var map in this.mapSheet)
            {
                this.mapIdByTerritoryTypeAndMapIndex.TryAdd((map.TerritoryType.RowId, map.MapIndex), map.RowId);
            }
        }

        var mapKey = (territoryTypeId, mapIndex);
        if (this.mapIdByTerritoryTypeAndMapIndex.ContainsKey(mapKey))
        {
            return this.mapIdByTerritoryTypeAndMapIndex[mapKey];
        }

        return 0;
    }

    private RowRef<Map> GetMapAtLayerIndex(TerritoryType territoryType, uint layerIndex)
    {
        this.npcLayerCache.TryAdd(territoryType.RowId, new Dictionary<uint, RowRef<Map>>());

        var rowRefs = this.npcLayerCache[territoryType.RowId];
        if (rowRefs.TryGetValue(layerIndex, out var value))
        {
            return value;
        }

        var mapId = this.GetMapIdByTerritoryTypeAndMapIndex(territoryType.RowId, (sbyte)layerIndex);
        if (mapId == 0)
        {
            if (rowRefs.Any())
            {
                var maxLayer = rowRefs.Max(c => c.Key);

                var actualLayer = ((layerIndex - 1) % maxLayer) + 1;
                if (rowRefs.ContainsKey(actualLayer) && rowRefs[actualLayer].RowId != 0)
                {
                    rowRefs[layerIndex] = rowRefs[actualLayer];
                    return rowRefs[layerIndex];
                }
            }
        }

        rowRefs[layerIndex] = new RowRef<Map>(this.gameData.Excel, mapId);
        return rowRefs[layerIndex];
    }

    public HashSet<NpcLocation>? GetLocations(uint npcId)
    {
        return this.locations.GetValueOrDefault(npcId);
    }

    public Dictionary<uint, HashSet<NpcLocation>> BuildLevelMap()
    {
        var npcLevelLookup = new Dictionary<uint, HashSet<NpcLocation>>();

        foreach (var sTerritoryType in this.territoryTypeSheet)
        {
            var bg = sTerritoryType.Bg.ToString();
            if (string.IsNullOrEmpty(bg))
            {
                continue;
            }

            var lgbFileName = "bg/" + bg.Substring(0, bg.IndexOf("/level/") + 1) + "level/planevent.lgb";
            var sLgbFile = this.gameData.GetFile<LgbFile>(lgbFileName);
            if (sLgbFile == null)
            {
                continue;
            }

            for (var index = 0u; index < sLgbFile.Layers.Length; index++)
            {
                var sLgbGroup = sLgbFile.Layers[index];
                var map = this.GetMapAtLayerIndex(sTerritoryType, index + 1);
                foreach (var instanceObject in sLgbGroup.InstanceObjects)
                {
                    if (instanceObject.AssetType == LayerEntryType.EventNPC)
                    {
                        var eventNpc = (LayerCommon.ENPCInstanceObject)instanceObject.Object;
                        var npcRowId = eventNpc.ParentData.ParentData.BaseId;
                        if (npcRowId != 0)
                        {
                            if (!npcLevelLookup.ContainsKey(npcRowId))
                            {
                                npcLevelLookup.Add(npcRowId, new HashSet<NpcLocation>());
                            }

                            if (map.RowId == 0)
                            {
                                continue;
                            }

                            var npcLocation = new NpcLocation(
                                instanceObject.Transform.Translation.X,
                                instanceObject.Transform.Translation.Z,
                                map.RowId != 0 && map.ValueNullable != null ? map : sTerritoryType.Map,
                                sTerritoryType.PlaceName,
                                new RowRef<TerritoryType>(this.gameData.Excel, sTerritoryType.RowId));
                            npcLevelLookup[npcRowId].Add(npcLocation);
                        }
                    }
                }
            }
        }

        foreach (var level in this.levelSheet
                     .Where(c => c.Object.RowId is > 1000000 and < 11000000))
        {
            var npcLocation = new NpcLocation(
                level.X,
                level.Z,
                level.Map,
                level.Map.Value.PlaceName,
                new RowRef<TerritoryType>(this.gameData.Excel, level.Territory.RowId));

            npcLevelLookup.TryAdd(level.Object.RowId, new HashSet<NpcLocation>());
            if (!npcLevelLookup[level.Object.RowId].Any(c => c.EqualRounded(npcLocation)))
            {
                npcLevelLookup[level.Object.RowId].Add(npcLocation);
            }
        }

        foreach (var npc in this.eNpcPlaces)
        {
            if (!npcLevelLookup.ContainsKey(npc.ENpcResidentId))
            {
                npcLevelLookup.Add(npc.ENpcResidentId, new HashSet<NpcLocation>());
            }

            if (npc.TerritoryType.ValueNullable != null)
            {
                var npcLocation = new NpcLocation(
                    npc.Position.X,
                    npc.Position.Y,
                    npc.TerritoryType.Value.Map,
                    npc.TerritoryType.Value.PlaceName,
                    npc.TerritoryType,
                    true);
                if (!npcLevelLookup[npc.ENpcResidentId].Any(c => c.EqualRounded(npcLocation)))
                {
                    npcLevelLookup[npc.ENpcResidentId].Add(npcLocation);
                }
            }
        }

        this.locations = npcLevelLookup;

        return npcLevelLookup;
    }
}