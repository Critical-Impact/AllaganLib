using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Service;
using Dalamud.Plugin;
using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Caches;

public class NpcShopCache : IDisposable
{
    public const string NpcIdToShopIdLookupName = "NpcIdToShopIdLookup";
    public const string ShopIdToNpcIdLookupName = "ShopIdToNpcIdLookup";
    public const string NpcIdToTripleTriadLookupName = "NpcIdToTripleTriadLookup";
    public const string TripleTriadToNpcIdLookupName = "TripleTriadToNpcIdLookup";
    public const string NpcIdToFateShopIdLookupName = "NpcIdToFateShopIdLookup";
    public const string NpcIdToSpecialShopIdLookupName = "NpcIdToSpecialShopIdLookup";
    public const string NpcIdToFccShopIdLookupName = "NpcIdToFccShopIdLookup";
    public const string NpcIdToGilShopIdLookupName = "NpcIdToGilShopIdLookup";
    public const string NpcIdToGcShopIdLookupName = "NpcIdToGcShopIdLookup";
    public const string NpcIdToInclusionShopIdLookupName = "NpcIdToInclusionShopIdLookup";
    public const string NpcIdToCollectablesShopIdLookupName = "NpcIdToCollectablesShopIdLookup";
    public const string NpcIdToAnimaShopIdLookupName = "NpcIdToAnimaShopIdLookup";
    public const string FateShopIdToNpcIdLookupName = "FateShopIdToNpcIdLookup";
    public const string SpecialShopIdToNpcIdLookupName = "SpecialShopIdToNpcIdLookup";
    public const string FccShopIdToNpcIdLookupName = "FccShopIdToNpcIdLookup";
    public const string GilShopIdToNpcIdLookupName = "GilShopIdToNpcIdLookup";
    public const string GcShopIdToNpcIdLookupName = "GcShopIdToNpcIdLookup";
    public const string InclusionShopIdToNpcLookupName = "InclusionShopIdToNpcLookup";
    public const string CollectablesShopIdToNpcLookupName = "CollectablesShopIdToNpcLookup";
    public const string AnimaShopIdToNpcIdLookupName = "AnimaShopIdToNpcIdLookup";

    private readonly IDalamudPluginInterface pluginInterface;
    private readonly GameData gameData;
    private readonly SheetManagerStartupOptions startupOptions;

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
    private Dictionary<uint, HashSet<uint>> npcIdToAnimaShopIdLookup;

    private Dictionary<uint, HashSet<uint>> fateShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> specialShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> fccShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> gilShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> gcShopIdToNpcIdLookup;
    private Dictionary<uint, HashSet<uint>> inclusionShopIdToNpcLookup;
    private Dictionary<uint, HashSet<uint>> collectablesShopIdToNpcLookup;
    private Dictionary<uint, HashSet<uint>> animaShopIdToNpcIdLookup;

    private IReadOnlyList<DataShareBinding<Dictionary<uint, HashSet<uint>>>> DataShareBindings
    =>
    [
        new(
                NpcIdToShopIdLookupName,
                () => this.npcIdToShopIdLookup,
                v => this.npcIdToShopIdLookup = v),

            new(
                ShopIdToNpcIdLookupName,
                () => this.shopIdToNpcIdLookup,
                v => this.shopIdToNpcIdLookup = v),

            new(
                NpcIdToTripleTriadLookupName,
                () => this.npcIdToTripleTriadLookup,
                v => this.npcIdToTripleTriadLookup = v),

            new(
                TripleTriadToNpcIdLookupName,
                () => this.tripleTriadToNpcIdLookup,
                v => this.tripleTriadToNpcIdLookup = v),

            new(
                NpcIdToFateShopIdLookupName,
                () => this.npcIdToFateShopIdLookup,
                v => this.npcIdToFateShopIdLookup = v),

            new(
                FateShopIdToNpcIdLookupName,
                () => this.fateShopIdToNpcIdLookup,
                v => this.fateShopIdToNpcIdLookup = v),

            new(
                NpcIdToSpecialShopIdLookupName,
                () => this.npcIdToSpecialShopIdLookup,
                v => this.npcIdToSpecialShopIdLookup = v),

            new(
                SpecialShopIdToNpcIdLookupName,
                () => this.specialShopIdToNpcIdLookup,
                v => this.specialShopIdToNpcIdLookup = v),

            new(
                NpcIdToFccShopIdLookupName,
                () => this.npcIdToFccShopIdLookup,
                v => this.npcIdToFccShopIdLookup = v),

            new(
                FccShopIdToNpcIdLookupName,
                () => this.fccShopIdToNpcIdLookup,
                v => this.fccShopIdToNpcIdLookup = v),

            new(
                NpcIdToGilShopIdLookupName,
                () => this.npcIdToGilShopIdLookup,
                v => this.npcIdToGilShopIdLookup = v),

            new(
                GilShopIdToNpcIdLookupName,
                () => this.gilShopIdToNpcIdLookup,
                v => this.gilShopIdToNpcIdLookup = v),

            new(
                NpcIdToGcShopIdLookupName,
                () => this.npcIdToGcShopIdLookup,
                v => this.npcIdToGcShopIdLookup = v),

            new(
                GcShopIdToNpcIdLookupName,
                () => this.gcShopIdToNpcIdLookup,
                v => this.gcShopIdToNpcIdLookup = v),

            new(
                NpcIdToInclusionShopIdLookupName,
                () => this.npcIdToInclusionShopIdLookup,
                v => this.npcIdToInclusionShopIdLookup = v),

            new(
                InclusionShopIdToNpcLookupName,
                () => this.inclusionShopIdToNpcLookup,
                v => this.inclusionShopIdToNpcLookup = v),

            new(
                NpcIdToCollectablesShopIdLookupName,
                () => this.npcIdToCollectablesShopIdLookup,
                v => this.npcIdToCollectablesShopIdLookup = v),

            new(
                CollectablesShopIdToNpcLookupName,
                () => this.collectablesShopIdToNpcLookup,
                v => this.collectablesShopIdToNpcLookup = v),

            new(
                NpcIdToAnimaShopIdLookupName,
                () => this.npcIdToAnimaShopIdLookup,
                v => this.npcIdToAnimaShopIdLookup = v),

            new(
                AnimaShopIdToNpcIdLookupName,
                () => this.animaShopIdToNpcIdLookup,
                v => this.animaShopIdToNpcIdLookup = v)
    ];

    public int Version { get; } = 1;

    public string DataShareTag { get; } = "AllaganLib.Data.NpcShopCache";


    public NpcShopCache(IDalamudPluginInterface pluginInterface, GameData gameData, SheetManagerStartupOptions startupOptions)
    {
        this.pluginInterface = pluginInterface;
        this.gameData = gameData;
        this.startupOptions = startupOptions;
        this.npcIdToShopIdLookup = [];
        this.shopIdToNpcIdLookup = [];
        this.npcIdToFateShopIdLookup = [];
        this.npcIdToSpecialShopIdLookup = [];
        this.npcIdToFccShopIdLookup = [];
        this.npcIdToGilShopIdLookup = [];
        this.npcIdToGcShopIdLookup = [];
        this.npcIdToInclusionShopIdLookup = [];
        this.npcIdToCollectablesShopIdLookup = [];
        this.npcIdToAnimaShopIdLookup = [];

        this.fateShopIdToNpcIdLookup = [];
        this.specialShopIdToNpcIdLookup = [];
        this.fccShopIdToNpcIdLookup = [];
        this.gilShopIdToNpcIdLookup = [];
        this.gcShopIdToNpcIdLookup = [];
        this.inclusionShopIdToNpcLookup = [];
        this.collectablesShopIdToNpcLookup = [];
        this.animaShopIdToNpcIdLookup = [];

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
        if (this.startupOptions.CacheInDataShare)
        {
            if (!this.LoadDataShare())
            {
                this.BuildData();
                this.CreateDataShare();
            }
        }
        else
        {
            this.BuildData();
        }
    }

    private string GetDataShareTag(string lookupName)
    {
        return this.DataShareTag + "." + lookupName + "." + this.Version;
    }

    private bool LoadDataShare()
    {
        foreach (var binding in this.DataShareBindings)
        {
            if (!this.pluginInterface.TryGetData(
                    this.GetDataShareTag(binding.Name),
                    out Dictionary<uint, HashSet<uint>>? value))
            {
                return false;
            }

            binding.Setter(value);
        }

        return true;
    }

    private void CreateDataShare()
    {
        foreach (var binding in this.DataShareBindings)
        {
            this.pluginInterface.GetOrCreateData(this.GetDataShareTag(binding.Name), binding.Getter);
        }
    }


    private void BuildData()
    {
        var npcBaseSheet = this.gameData.GetExcelSheet<ENpcBase>()!;
        var fateShopSheet = this.gameData.GetExcelSheet<FateShop>()!;
        var fccShopSheet = this.gameData.GetExcelSheet<FccShop>()!;
        var specialShopSheet = this.gameData.GetExcelSheet<SpecialShop>()!;
        var topicSelectSheet = this.gameData.GetExcelSheet<TopicSelect>()!;
        var preHandlerSheet = this.gameData.GetExcelSheet<PreHandler>()!;
        var customTalkSheet = this.gameData.GetExcelSheet<CustomTalk>()!;
        var animaTradeItems = this.gameData.GetExcelSheet<AnimaWeapon5TradeItem>()!;

        foreach (var animaTradeItem in animaTradeItems)
        {
            var ulanId = 1017108u;
            this.animaShopIdToNpcIdLookup.TryAdd(animaTradeItem.RowId, []);
            this.animaShopIdToNpcIdLookup[animaTradeItem.RowId].Add(ulanId);

            this.shopIdToNpcIdLookup.TryAdd(animaTradeItem.RowId, []);
            this.shopIdToNpcIdLookup[animaTradeItem.RowId].Add(ulanId);

            this.npcIdToAnimaShopIdLookup.TryAdd(ulanId, []);
            this.npcIdToAnimaShopIdLookup[ulanId].Add(animaTradeItem.RowId);

            this.npcIdToShopIdLookup.TryAdd(ulanId, []);
            this.npcIdToShopIdLookup[ulanId].Add(animaTradeItem.RowId);
        }

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

        var npcToShopIdCount = this.npcIdToShopIdLookup.Count;
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

    public HashSet<uint>? GetAnimaShopsByNpcId(uint npcId)
    {
        return this.npcIdToAnimaShopIdLookup.GetValueOrDefault(npcId);
    }

    public HashSet<uint>? GetNpcsByAnimaShopId(uint animaShopId)
    {
        return this.animaShopIdToNpcIdLookup.GetValueOrDefault(animaShopId);
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

    public void Dispose()
    {
        if (!this.startupOptions.PersistInDataShare)
        {
            foreach (var binding in this.DataShareBindings)
            {
                this.pluginInterface.RelinquishData(this.GetDataShareTag(binding.Name));
            }
        }
    }
}