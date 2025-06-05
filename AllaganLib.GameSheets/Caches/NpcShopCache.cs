using System;
using System.Collections.Generic;
using System.Linq;
using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Caches;

public class NpcShopCache
{
    private readonly GameData gameData;
    private Dictionary<uint, HashSet<uint>> npcIdToShopIdLookup;
    private Dictionary<uint, HashSet<uint>> shopIdToNpcIdLookup;

    private Dictionary<uint, HashSet<uint>> npcIdToTripleTriadLookup;
    private Dictionary<uint, HashSet<uint>> tripleTriadToNpcIdLookup;

    private Dictionary<uint, HashSet<uint>> npcIdToFateShopIdLookup;
    private Dictionary<uint, HashSet<uint>> npcIdToSpecialShopIdLookup;
    private Dictionary<uint, HashSet<uint>> npcIdToFccShopIdLookup;
    private Dictionary<uint, HashSet<uint>> npcIdToGilShopIdLookup;
    private Dictionary<uint, HashSet<uint>> npcIdToGcShopIdLookup;
    private Dictionary<uint, HashSet<uint>> npcIdToInclusionShopIdLookup;
    private Dictionary<uint, HashSet<uint>> npcIdToCollectablesShopIdLookup;

    private Dictionary<uint, HashSet<uint>> fateShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> specialShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> fccShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> gilShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> gcShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> inclusionShopIdToNpcLookup;
    private Dictionary<uint, HashSet<uint>> collectablesShopIdToNpcLookup;


    public NpcShopCache(GameData gameData)
    {
        this.gameData = gameData;
        this.npcIdToShopIdLookup = [];
        this.shopIdToNpcIdLookup = [];
        this.npcIdToFateShopIdLookup = [];
        this.npcIdToSpecialShopIdLookup = [];
        this.npcIdToFccShopIdLookup = [];
        this.npcIdToGilShopIdLookup = [];
        this.npcIdToGcShopIdLookup = [];
        this.npcIdToInclusionShopIdLookup = [];
        this.npcIdToCollectablesShopIdLookup = [];

        this.fateShopIdToNpcIdLookup = [];
        this.specialShopIdToNpcIdLookup = [];
        this.fccShopIdToNpcIdLookup = [];
        this.gilShopIdToNpcIdLookup = [];
        this.gcShopIdToNpcIdLookup = [];
        this.inclusionShopIdToNpcLookup = [];
        this.collectablesShopIdToNpcLookup = [];

        this.npcIdToTripleTriadLookup = [];
        this.tripleTriadToNpcIdLookup = [];
    }

    private static readonly List<(uint, uint)> ExtraSpecialShops = new()
    {
        (1027998, 1769957),
        (1027538, 1769958),
        (1027385, 1769959),
        (1027497, 1769960),
        (1027892, 1769961),
        (1027665, 1769962),
        (1027709, 1769963),
        (1027766, 1769964),
        (1018655, 1769743),
        (1018655, 1769744),
        (1018655, 1770537),
        (1016289, 1769635),
        (1025047, 1769820),
        (1025047, 1769821),
        (1025047, 1769822),
        (1025047, 1769823),
        (1025047, 1769824),
        (1025047, 1769825),
        (1025047, 1769826),
        (1025047, 1769827),
        (1025047, 1769828),
        (1025047, 1769829),
        (1025047, 1769830),
        (1025047, 1769831),
        (1025047, 1769832),
        (1025047, 1769833),
        (1025047, 1769834),
        (1027123, 1769934),
        (1027123, 1769935),
        (1033921, 1770282),
        (1036895, 1770087),
        (1034007, 1770087),
    };

    private static readonly Dictionary<uint, uint> ExtraGilShops = new()
    {
        { 1025763, 262919 },
    };

    public void BuildDataMap()
    {
        var npcBaseSheet = this.gameData.GetExcelSheet<ENpcBase>()!;
        var fateShopSheet = this.gameData.GetExcelSheet<FateShop>()!;
        var fccShopSheet = this.gameData.GetExcelSheet<FccShop>()!;
        var specialShopSheet = this.gameData.GetExcelSheet<SpecialShop>()!;
        var topicSelectSheet = this.gameData.GetExcelSheet<TopicSelect>()!;
        var preHandlerSheet = this.gameData.GetExcelSheet<PreHandler>()!;
        var customTalkSheet = this.gameData.GetExcelSheet<CustomTalk>()!;
        var fateShopLookup = fateShopSheet.Select(c => c.RowId).ToHashSet();

        ReadOnlySpan<Type> customTalkTypes = [typeof(FateShop), typeof(FccShop), typeof(SpecialShop)];
        var customTalkTypeHash = RowRef.CreateTypeHash(customTalkTypes);

        foreach (var extraSpecialShop in ExtraSpecialShops)
        {
            var npcBase = npcBaseSheet.GetRow(extraSpecialShop.Item1);
            var specialShopRef = RowRef.Create<SpecialShop>(this.gameData.Excel, extraSpecialShop.Item2);
            EvalulateRowRef(npcBase, specialShopRef, customTalkTypes);
        }

        foreach (var extraGilShop in ExtraGilShops)
        {
            var npcBase = npcBaseSheet.GetRow(extraGilShop.Key);
            var specialShopRef = RowRef.Create<GilShop>(this.gameData.Excel, extraGilShop.Value);
            EvalulateRowRef(npcBase, specialShopRef, customTalkTypes);
        }

        foreach (var npc in npcBaseSheet)
        {
            // FateShops have the same IDs as their corresponding NPCs, they then have a references to their special shops
            if (fateShopLookup.Contains(npc.RowId))
            {
                var fateShop = fateShopSheet.GetRow(npc.RowId);
                var specialShops = fateShop.SpecialShop.Where(c => c.RowId != 0).Select(c => (RowRef)c).ToList();
                foreach (var specialShop in specialShops)
                {
                    EvalulateRowRef(npc, specialShop, customTalkTypes);
                }
            }

            foreach (var variable in npc.ENpcData)
            {
                EvalulateRowRef(npc, variable, customTalkTypes);
            }
        }

        var npcToShopIdCount = npcIdToShopIdLookup.Count;
        var shopIdToNpcIdCount = this.shopIdToNpcIdLookup.Count;

        void EvalulateRowRef(ENpcBase npcBase, RowRef rowRef, ReadOnlySpan<Type> customTalkTypes)
        {
            if (rowRef.Is<FccShop>())
            {
                this.fccShopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.fccShopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.shopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.shopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.npcIdToFccShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToFccShopIdLookup[npcBase.RowId].Add(rowRef.RowId);

                this.npcIdToShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToShopIdLookup[npcBase.RowId].Add(rowRef.RowId);
            }
            else if (rowRef.Is<GCShop>())
            {
                this.gcShopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.gcShopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.shopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.shopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.npcIdToGcShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToGcShopIdLookup[npcBase.RowId].Add(rowRef.RowId);

                this.npcIdToShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToShopIdLookup[npcBase.RowId].Add(rowRef.RowId);
            }
            else if (rowRef.Is<GilShop>())
            {
                this.gilShopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.gilShopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.shopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.shopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.npcIdToGilShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToGilShopIdLookup[npcBase.RowId].Add(rowRef.RowId);

                this.npcIdToShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToShopIdLookup[npcBase.RowId].Add(rowRef.RowId);
            }
            else if (rowRef.Is<SpecialShop>())
            {
                this.specialShopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.specialShopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.shopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.shopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.npcIdToSpecialShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToSpecialShopIdLookup[npcBase.RowId].Add(rowRef.RowId);

                this.npcIdToShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToShopIdLookup[npcBase.RowId].Add(rowRef.RowId);
            }
            else if (rowRef.Is<InclusionShop>())
            {
                this.inclusionShopIdToNpcLookup.TryAdd(rowRef.RowId, []);
                this.inclusionShopIdToNpcLookup[rowRef.RowId].Add(npcBase.RowId);

                this.shopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.shopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.npcIdToInclusionShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToInclusionShopIdLookup[npcBase.RowId].Add(rowRef.RowId);

                this.npcIdToShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToShopIdLookup[npcBase.RowId].Add(rowRef.RowId);
            }
            else if (rowRef.Is<CollectablesShop>())
            {
                this.collectablesShopIdToNpcLookup.TryAdd(rowRef.RowId, []);
                this.collectablesShopIdToNpcLookup[rowRef.RowId].Add(npcBase.RowId);

                this.shopIdToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.shopIdToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.npcIdToCollectablesShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToCollectablesShopIdLookup[npcBase.RowId].Add(rowRef.RowId);

                this.npcIdToShopIdLookup.TryAdd(npcBase.RowId, []);
                this.npcIdToShopIdLookup[npcBase.RowId].Add(rowRef.RowId);
            }
            else if (rowRef.Is<CustomTalk>())
            {
                var customTalk = customTalkSheet.GetRow(rowRef.RowId);
                EvalulateRowRef(npcBase, customTalk.SpecialLinks, customTalkTypes);
                foreach (var scriptStruct in customTalk.Script)
                {
                    if (scriptStruct.ScriptArg == 0)
                    {
                        continue;
                    }
                    var customTalkRef = RowRef.GetFirstValidRowOrUntyped(
                        this.gameData.Excel,
                        scriptStruct.ScriptArg,
                        customTalkTypes,
                        customTalkTypeHash,
                        this.gameData.Options.DefaultExcelLanguage);
                    EvalulateRowRef(npcBase, customTalkRef, customTalkTypes);
                }
            }
            else if (rowRef.Is<TopicSelect>())
            {
                var topicSelect = topicSelectSheet.GetRow(rowRef.RowId);
                foreach (var topicShop in topicSelect.Shop)
                {
                    EvalulateRowRef(npcBase, topicShop, customTalkTypes);
                }
            }
            else if(rowRef.Is<PreHandler>())
            {
                var preHandler = preHandlerSheet.GetRowOrDefault(rowRef.RowId);
                if (preHandler != null)
                {
                    EvalulateRowRef(npcBase, preHandler.Value.Target, customTalkTypes);
                }
            }
            else if(rowRef.Is<TripleTriad>())
            {
                this.tripleTriadToNpcIdLookup.TryAdd(rowRef.RowId, []);
                this.tripleTriadToNpcIdLookup[rowRef.RowId].Add(npcBase.RowId);

                this.npcIdToTripleTriadLookup.TryAdd(rowRef.RowId, []);
                this.npcIdToTripleTriadLookup[rowRef.RowId].Add(npcBase.RowId);
            }
        }
    }

    public HashSet<uint>? GetGcShopsByNpcId(uint npcId)
    {
        return this.npcIdToGcShopIdLookup.GetValueOrDefault(npcId);
    }

    public HashSet<uint>? GetNpcsByGcShopId(uint gcShopId)
    {
        return this.gcShopIdToNpcIdLookup.GetValueOrDefault(gcShopId);
    }

    public HashSet<uint>? GetGilShopsByNpcId(uint npcId)
    {
        return this.npcIdToGilShopIdLookup.GetValueOrDefault(npcId);
    }

    public HashSet<uint>? GetNpcsByGilShopId(uint gilShopId)
    {
        return this.gilShopIdToNpcIdLookup.GetValueOrDefault(gilShopId);
    }

    public HashSet<uint>? GetSpecialShopsByNpcId(uint npcId)
    {
        return this.npcIdToSpecialShopIdLookup.GetValueOrDefault(npcId);
    }

    public HashSet<uint>? GetNpcsBySpecialShopId(uint specialShopId)
    {
        return this.specialShopIdToNpcIdLookup.GetValueOrDefault(specialShopId);
    }


    public HashSet<uint>? GetFccShopsByNpcId(uint npcId)
    {
        return this.npcIdToFccShopIdLookup.GetValueOrDefault(npcId);
    }

    public HashSet<uint>? GetNpcsByFccShopId(uint fccShopId)
    {
        return this.fccShopIdToNpcIdLookup.GetValueOrDefault(fccShopId);
    }


    public HashSet<uint>? GetInclusionShopsByNpcId(uint npcId)
    {
        return this.npcIdToInclusionShopIdLookup.GetValueOrDefault(npcId);
    }

    public HashSet<uint>? GetNpcsByInclusionShopId(uint inclusionShopId)
    {
        return this.inclusionShopIdToNpcLookup.GetValueOrDefault(inclusionShopId);
    }

    public HashSet<uint>? GetCollectablesShopsByNpcId(uint npcId)
    {
        return this.npcIdToCollectablesShopIdLookup.GetValueOrDefault(npcId);
    }

    public HashSet<uint>? GetNpcsByCollectablesShopId(uint collectablesShopId)
    {
        return this.collectablesShopIdToNpcLookup.GetValueOrDefault(collectablesShopId);
    }

    public HashSet<uint>? GetTripleTriadsByNpcId(uint npcId)
    {
        return this.npcIdToTripleTriadLookup.GetValueOrDefault(npcId);
    }

    public HashSet<uint>? GetNpcsByTripleTriadId(uint tripleTriadId)
    {
        return this.tripleTriadToNpcIdLookup.GetValueOrDefault(tripleTriadId);
    }


    public HashSet<uint>? GetShops(uint npcId)
    {
        this.npcIdToShopIdLookup.TryGetValue(npcId, out var shops);
        return shops;
    }

    public HashSet<uint>? GetNpcIds(uint shopId)
    {
        this.shopIdToNpcIdLookup.TryGetValue(shopId, out var npcIds);
        return npcIds;
    }
}