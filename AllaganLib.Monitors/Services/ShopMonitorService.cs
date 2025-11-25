using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Monitors.Enums;
using AllaganLib.Monitors.Interfaces;
using AllaganLib.Shared.Extensions;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AllaganLib.Monitors.Services;

/// <inheritdoc cref="IShopMonitorService" />
public class ShopMonitorService : IHostedService, IDisposable, IShopMonitorService
{
    private readonly ILogger<ShopMonitorService> logger;
    private readonly ITargetManager targetManager;
    private readonly GilShopSheet gilShopSheet;
    private readonly SpecialShopSheet specialShopSheet;
    private readonly CollectablesShopSheet collectablesShopSheet;
    private readonly InclusionShopSheet inclusionShopSheet;
    private readonly FccShopSheet fccShopSheet;
    private readonly ENpcBaseSheet enpcBaseSheet;
    private readonly IAddonLifecycle addonLifecycle;

    private readonly HashSet<uint> specialShops = [];
    private readonly HashSet<uint> gilShops = [];
    private readonly HashSet<uint> inclusionShops = [];
    private readonly HashSet<uint> collectableShops = [];
    private readonly HashSet<uint> fccShops = [];

    private readonly Dictionary<uint, uint> gilShopPreHandlers = new();
    private readonly Dictionary<uint, uint> specialShopPreHandlers = new();
    private readonly Dictionary<uint, uint> collectableShopPreHandlers = new();
    private readonly Dictionary<uint, uint> inclusionShopPreHandlers = new();

    private readonly Dictionary<uint, HashSet<uint>> gilShopCustomTalk = new();
    private readonly Dictionary<uint, HashSet<uint>> specialShopCustomTalk = new();
    private readonly Dictionary<uint, HashSet<uint>> collectableShopCustomTalk = new();
    private readonly Dictionary<uint, HashSet<uint>> inclusionShopCustomTalk = new();
    private readonly Dictionary<uint, HashSet<uint>> fccShopCustomTalk = new();

    private readonly Dictionary<uint, HashSet<uint>> gilShopTopicSelect = new();
    private readonly Dictionary<uint, HashSet<uint>> specialShopTopicSelect = new();
    private readonly Dictionary<uint, HashSet<uint>> collectableShopTopicSelect = new();
    private readonly Dictionary<uint, HashSet<uint>> inclusionShopTopicSelect = new();

    private readonly Dictionary<uint, HashSet<uint>> collectableShopSpecialLink = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopMonitorService"/> class.
    /// </summary>
    /// <param name="logger">A logger.</param>
    /// <param name="dataManager">Dalamud's data manager.</param>
    /// <param name="targetManager">Dalamud's target manager.</param>
    /// <param name="gilShopSheet">AllaganLib.GameData's gil shop sheet.</param>
    /// <param name="specialShopSheet">AllaganLib.GameData's special shop sheet.</param>
    /// <param name="collectablesShopSheet">AllaganLib.GameData's collectables shop sheet.</param>
    /// <param name="inclusionShopSheet">AllaganLib.GameData's inclusion shop sheet.</param>
    /// <param name="fccShopSheet">AllaganLib.GameData's fcc shop sheet.</param>
    /// <param name="enpcBaseSheet">AllaganLib.GameData's enpc base sheet.</param>
    /// <param name="addonLifecycle">Dalamud's addon lifecycle service.</param>
    public ShopMonitorService(ILogger<ShopMonitorService> logger, IDataManager dataManager, ITargetManager targetManager, GilShopSheet gilShopSheet, SpecialShopSheet specialShopSheet, CollectablesShopSheet collectablesShopSheet, InclusionShopSheet inclusionShopSheet, FccShopSheet fccShopSheet, ENpcBaseSheet enpcBaseSheet, IAddonLifecycle addonLifecycle)
    {
        this.logger = logger;
        this.targetManager = targetManager;
        this.gilShopSheet = gilShopSheet;
        this.specialShopSheet = specialShopSheet;
        this.collectablesShopSheet = collectablesShopSheet;
        this.inclusionShopSheet = inclusionShopSheet;
        this.fccShopSheet = fccShopSheet;
        this.enpcBaseSheet = enpcBaseSheet;
        this.addonLifecycle = addonLifecycle;
        foreach (var item in dataManager.GetExcelSheet<SpecialShop>())
        {
            this.specialShops.Add(item.RowId);
        }

        foreach (var item in dataManager.GetExcelSheet<GilShop>())
        {
            this.gilShops.Add(item.RowId);
        }

        foreach (var item in dataManager.GetExcelSheet<InclusionShop>())
        {
            this.inclusionShops.Add(item.RowId);
        }

        foreach (var item in dataManager.GetExcelSheet<Lumina.Excel.Sheets.CollectablesShop>())
        {
            this.collectableShops.Add(item.RowId);
        }

        foreach (var item in dataManager.GetExcelSheet<Lumina.Excel.Sheets.FccShop>())
        {
            this.fccShops.Add(item.RowId);
        }

        ReadOnlySpan<Type> readOnlySpan = [typeof(Lumina.Excel.Sheets.CollectablesShop), typeof(InclusionShop), typeof(GilShop), typeof(SpecialShop), typeof(FccShop)];
        var typeHash = RowRef.CreateTypeHash(readOnlySpan);

        foreach (var customTalk in dataManager.GetExcelSheet<CustomTalk>())
        {
            if (customTalk.SpecialLinks.Is<SpecialShop>())
            {
                this.specialShops.Add(customTalk.SpecialLinks.RowId);
                this.specialShopCustomTalk.TryAdd(customTalk.RowId, []);
            }

            if (customTalk.SpecialLinks.Is<CollectablesShop>())
            {
                this.collectableShopSpecialLink.TryAdd(customTalk.RowId, []);
                this.collectableShopSpecialLink[customTalk.RowId].Add(customTalk.SpecialLinks.RowId);
            }

            foreach (var scriptStruct in customTalk.Script)
            {
                var rowRef = RowRef.GetFirstValidRowOrUntyped(dataManager.Excel, scriptStruct.ScriptArg, readOnlySpan, typeHash, dataManager.GameData.Options.DefaultExcelLanguage);
                if (rowRef.Is<Lumina.Excel.Sheets.CollectablesShop>())
                {
                    this.collectableShops.Add(rowRef.RowId);
                    this.collectableShopCustomTalk.TryAdd(customTalk.RowId, []);
                    this.collectableShopCustomTalk[customTalk.RowId].Add(rowRef.RowId);
                }
                else if (rowRef.Is<Lumina.Excel.Sheets.InclusionShop>())
                {
                    this.inclusionShops.Add(rowRef.RowId);
                    this.inclusionShopCustomTalk.TryAdd(customTalk.RowId, []);
                    this.inclusionShopCustomTalk[customTalk.RowId].Add(rowRef.RowId);
                }
                else if (rowRef.Is<Lumina.Excel.Sheets.GilShop>())
                {
                    this.gilShops.Add(rowRef.RowId);
                    this.gilShopCustomTalk.TryAdd(customTalk.RowId, []);
                    this.gilShopCustomTalk[customTalk.RowId].Add(rowRef.RowId);
                }
                else if (rowRef.Is<Lumina.Excel.Sheets.SpecialShop>())
                {
                    this.specialShops.Add(rowRef.RowId);
                    this.specialShopCustomTalk.TryAdd(customTalk.RowId, []);
                    this.specialShopCustomTalk[customTalk.RowId].Add(rowRef.RowId);
                }
                else if (rowRef.Is<Lumina.Excel.Sheets.FccShop>())
                {
                    this.fccShops.Add(rowRef.RowId);
                    this.fccShopCustomTalk.TryAdd(customTalk.RowId, []);
                    this.fccShopCustomTalk[customTalk.RowId].Add(rowRef.RowId);
                }
            }
        }

        foreach (var preHandler in dataManager.GetExcelSheet<PreHandler>())
        {
            if (preHandler.Target.Is<Lumina.Excel.Sheets.CollectablesShop>())
            {
                this.collectableShopPreHandlers[preHandler.RowId] = preHandler.Target.RowId;
            }
            else if (preHandler.Target.Is<Lumina.Excel.Sheets.InclusionShop>())
            {
                this.inclusionShopPreHandlers[preHandler.RowId] = preHandler.Target.RowId;
            }
            else if (preHandler.Target.Is<Lumina.Excel.Sheets.GilShop>())
            {
                this.gilShopPreHandlers[preHandler.RowId] = preHandler.Target.RowId;
            }
            else if (preHandler.Target.Is<Lumina.Excel.Sheets.SpecialShop>())
            {
                this.specialShopPreHandlers[preHandler.RowId] = preHandler.Target.RowId;
            }
        }

        foreach (var topicSelect in dataManager.GetExcelSheet<TopicSelect>())
        {
            foreach (var shop in topicSelect.Shop)
            {
                if (shop.Is<Lumina.Excel.Sheets.PreHandler>())
                {
                    var preHandler = shop.GetValueOrDefault<PreHandler>();
                    if (preHandler != null)
                    {
                        if (preHandler.Value.Target.Is<Lumina.Excel.Sheets.CollectablesShop>())
                        {
                            this.collectableShopTopicSelect.TryAdd(preHandler.Value.RowId, []);
                            this.collectableShopTopicSelect[preHandler.Value.RowId].Add(preHandler.Value.Target.RowId);
                        }
                        else if (preHandler.Value.Target.Is<Lumina.Excel.Sheets.InclusionShop>())
                        {
                            this.inclusionShopTopicSelect.TryAdd(preHandler.Value.RowId, []);
                            this.inclusionShopTopicSelect[preHandler.Value.RowId].Add(preHandler.Value.Target.RowId);
                        }
                        else if (preHandler.Value.Target.Is<Lumina.Excel.Sheets.GilShop>())
                        {
                            this.gilShopTopicSelect.TryAdd(preHandler.Value.RowId, []);
                            this.gilShopTopicSelect[preHandler.Value.RowId].Add(preHandler.Value.Target.RowId);
                        }
                        else if (preHandler.Value.Target.Is<Lumina.Excel.Sheets.SpecialShop>())
                        {
                            this.specialShopTopicSelect.TryAdd(preHandler.Value.RowId, []);
                            this.specialShopTopicSelect[preHandler.Value.RowId].Add(preHandler.Value.Target.RowId);
                        }
                    }
                }
                else if (shop.Is<Lumina.Excel.Sheets.GilShop>())
                {
                    this.gilShopTopicSelect.TryAdd(topicSelect.RowId, []);
                    this.gilShopTopicSelect[topicSelect.RowId].Add(shop.RowId);
                }
                else if (shop.Is<Lumina.Excel.Sheets.SpecialShop>())
                {
                    this.specialShopTopicSelect.TryAdd(topicSelect.RowId, []);
                    this.specialShopTopicSelect[topicSelect.RowId].Add(shop.RowId);
                }
            }
        }
    }

    /// <inheritdoc/>
    public event IShopMonitorService.ShopChangedDelegate? OnShopChanged;

    /// <inheritdoc/>
    public event IShopMonitorService.ShopOpenedDelegate? OnShopOpened;

    /// <inheritdoc/>
    public event IShopMonitorService.ShopClosedDelegate? OnShopClosed;

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.logger.LogTrace("Starting service {type} ({this})", this.GetType().Name, this);
        this.addonLifecycle.RegisterListener(AddonEvent.PostSetup, ["Shop", "FreeCompanyCreditShop", "ShopExchangeItem", "ShopExchangeCurrency", "InclusionShop", "CollectablesShop"], this.AddonPostSetup);
        this.addonLifecycle.RegisterListener(AddonEvent.PreFinalize, ["Shop", "FreeCompanyCreditShop", "ShopExchangeItem", "ShopExchangeCurrency", "InclusionShop", "CollectablesShop"], this.AddonPreFinalize);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogTrace("Stopping service {type} ({this})", this.GetType().Name, this);
        this.addonLifecycle.UnregisterListener(AddonEvent.PostSetup, ["Shop", "FreeCompanyCreditShop", "ShopExchangeItem", "ShopExchangeCurrency", "InclusionShop", "CollectablesShop"], this.AddonPostSetup);
        this.addonLifecycle.UnregisterListener(AddonEvent.PreFinalize, ["Shop", "FreeCompanyCreditShop", "ShopExchangeItem", "ShopExchangeCurrency", "InclusionShop", "CollectablesShop"], this.AddonPreFinalize);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public (ENpcBaseRow Npc, List<IShop> Shops, IShop? ActiveShop)? GetCurrentShopType()
    {
        var currentShopIds = this.GetCurrentShopTypeIds();
        if (currentShopIds == null)
        {
            return null;
        }

        var shopIds = currentShopIds.Value.Shops;
        var activeShopId = currentShopIds.Value.ActiveShopId;

        var shops = new List<IShop>();
        IShop? activeShop = null;
        var enpcBaseRow = this.enpcBaseSheet.GetRowOrDefault(currentShopIds.Value.NpcId);
        if (enpcBaseRow == null)
        {
            return null;
        }

        foreach (var shopId in shopIds)
        {
            var shop = this.GetShopByIdAndType(shopId.Item2, shopId.Item1);
            if (shop != null)
            {
                shops.Add(shop);
            }
        }

        if (activeShopId != null)
        {
            var shop = this.GetShopByIdAndType(activeShopId.Value.Item2, activeShopId.Value.Item1);
            if (shop != null)
            {
                activeShop = shop;
            }
        }

        return (enpcBaseRow, shops, activeShop);
    }

    /// <inheritdoc/>
    public IShop? GetShopByIdAndType(uint shopId, ShopType type)
    {
        switch (type)
        {
            case ShopType.Gil:
                var gilShop = this.gilShopSheet.GetRowOrDefault(shopId);
                if (gilShop != null)
                {
                    return gilShop;
                }

                break;
            case ShopType.SpecialShop:
                var specialShop = this.specialShopSheet.GetRowOrDefault(shopId);
                if (specialShop != null)
                {
                    return specialShop;
                }

                break;
            case ShopType.Collectable:
                var collectablesShop = this.collectablesShopSheet.GetRowOrDefault(shopId);
                if (collectablesShop != null)
                {
                    return collectablesShop;
                }

                break;
            case ShopType.InclusionShop:
                var inclusionShop = this.inclusionShopSheet.GetRowOrDefault(shopId);
                if (inclusionShop != null)
                {
                    return inclusionShop;
                }

                break;
            case ShopType.FreeCompanyShop:
                var fccShop = this.fccShopSheet.GetRowOrDefault(shopId);
                if (fccShop != null)
                {
                    return fccShop;
                }

                break;
        }

        return null;
    }

    /// <inheritdoc/>
    public unsafe (uint NpcId, List<(ShopType, uint)> Shops, (ShopType, uint)? ActiveShopId)? GetCurrentShopTypeIds()
    {
        var eventFramework = EventFramework.Instance();
        if (eventFramework != null)
        {
            uint? npcId = null;
            List<(ShopType, uint)> shops = [];
            (ShopType, uint)? shopId = null;

            foreach (var eventHandler in eventFramework->EventHandlerModule.EventHandlerMap)
            {
                if (eventHandler.Item2.Value != null)
                {
                    var activeTarget = false;
                    foreach (var eventObject in eventHandler.Item2.Value->EventObjects)
                    {
                        if (this.targetManager.Target?.DataId == eventObject.Value->BaseId)
                        {
                            npcId = this.targetManager.Target?.DataId;
                            activeTarget = true;
                        }
                    }

                    if (!activeTarget)
                    {
                        continue;
                    }

                    if (this.collectableShops.Contains(eventHandler.Item1))
                    {
                        shops.Add((ShopType.Collectable, eventHandler.Item1));
                    }
                    else if (this.inclusionShops.Contains(eventHandler.Item1))
                    {
                        shops.Add((ShopType.InclusionShop, eventHandler.Item1));
                    }
                    else if (this.gilShops.Contains(eventHandler.Item1))
                    {
                        shops.Add((ShopType.Gil, eventHandler.Item1));
                    }
                    else if (this.specialShops.Contains(eventHandler.Item1))
                    {
                        shops.Add((ShopType.SpecialShop, eventHandler.Item1));
                    }
                    else if (this.fccShops.Contains(eventHandler.Item1))
                    {
                        shops.Add((ShopType.FreeCompanyShop, eventHandler.Item1));
                    }
                    else if (this.collectableShopCustomTalk.TryGetValue(eventHandler.Item1, out var value))
                    {
                        foreach (var customTalkShopId in value)
                        {
                            shops.Add((ShopType.Collectable, customTalkShopId));
                        }
                    }
                    else if (this.inclusionShopCustomTalk.TryGetValue(eventHandler.Item1, out var value1))
                    {
                        foreach (var customTalkShopId in value1)
                        {
                            shops.Add((ShopType.InclusionShop, customTalkShopId));
                        }
                    }
                    else if (this.gilShopCustomTalk.TryGetValue(eventHandler.Item1, out var value2))
                    {
                        foreach (var customTalkShopId in value2)
                        {
                            shops.Add((ShopType.Gil, customTalkShopId));
                        }
                    }
                    else if (this.specialShopCustomTalk.TryGetValue(eventHandler.Item1, out var value3))
                    {
                        foreach (var customTalkShopId in value3)
                        {
                            shops.Add((ShopType.SpecialShop, customTalkShopId));
                        }
                    }
                    else if (this.fccShopCustomTalk.TryGetValue(eventHandler.Item1, out var value4))
                    {
                        foreach (var customTalkShopId in value4)
                        {
                            shops.Add((ShopType.FreeCompanyShop, customTalkShopId));
                        }
                    }
                    else if (this.collectableShopPreHandlers.TryGetValue(eventHandler.Item1, out var handler))
                    {
                        shops.Add((ShopType.Collectable, handler));
                    }
                    else if (this.inclusionShopPreHandlers.TryGetValue(eventHandler.Item1, out var preHandler))
                    {
                        shops.Add((ShopType.InclusionShop, preHandler));
                    }
                    else if (this.gilShopPreHandlers.TryGetValue(eventHandler.Item1, out var shopPreHandler))
                    {
                        shops.Add((ShopType.Gil, shopPreHandler));
                    }
                    else if (this.specialShopPreHandlers.TryGetValue(eventHandler.Item1, out var specialShopPreHandler))
                    {
                        shops.Add((ShopType.SpecialShop, specialShopPreHandler));
                    }
                    else if (this.collectableShopTopicSelect.TryGetValue(eventHandler.Item1, out var value5))
                    {
                        foreach (var topicSelectShopId in value5)
                        {
                            shops.Add((ShopType.Collectable, topicSelectShopId));
                        }
                    }
                    else if (this.inclusionShopTopicSelect.TryGetValue(eventHandler.Item1, out var value6))
                    {
                        foreach (var topicSelectShopId in value6)
                        {
                            shops.Add((ShopType.InclusionShop, topicSelectShopId));
                        }
                    }
                    else if (this.gilShopTopicSelect.TryGetValue(eventHandler.Item1, out var value7))
                    {
                        foreach (var topicSelectShopId in value7)
                        {
                            shops.Add((ShopType.Gil, topicSelectShopId));
                        }
                    }
                    else if (this.specialShopTopicSelect.TryGetValue(eventHandler.Item1, out var value8))
                    {
                        foreach (var topicSelectShopId in value8)
                        {
                            shops.Add((ShopType.SpecialShop, topicSelectShopId));
                        }
                    }
                    else if (this.collectableShopSpecialLink.TryGetValue(eventHandler.Item1, out var value9))
                    {
                        foreach (var collectableShopId in value9)
                        {
                            shops.Add((ShopType.Collectable, collectableShopId));
                        }
                    }

                    if (activeTarget)
                    {
                         var freeCompanyShopAgent = UIModule.Instance()->GetAgentModule()->GetAgentByInternalId(AgentId.FreeCompanyCreditShop);
                         if (freeCompanyShopAgent != null && freeCompanyShopAgent->IsAgentActive() && eventHandler.Item2.Value != null && eventHandler.Item2.Value->Info.EventId.ContentId == EventHandlerContent.FreeCompanyCreditShop)
                         {
                             shopId = shops.FirstOrDefault(c => c.Item2 == eventHandler.Item1);
                         }
                         else if (shops.Count == 1 && shops.All(c => c.Item1 == ShopType.Collectable))
                         {
                             shopId = shops.First();
                         }
                         else if (shops.Count == 1 && shops.All(c => c.Item1 == ShopType.InclusionShop))
                         {
                             shopId = shops.First();
                         }
                         else
                         {
                             var agent = (AgentShop*)UIModule.Instance()->GetAgentModule()->GetAgentByInternalId(AgentId.Shop);
                             if (agent != null && agent->IsAgentActive() && agent->EventReceiver != null &&
                                 agent->IsAddonReady())
                             {
                                 var proxy = (ShopEventHandler.AgentProxy*)agent->EventReceiver;
                                 if (proxy != null && proxy->Handler != null)
                                 {
                                     shopId = shops.FirstOrDefault(c => c.Item2 == proxy->Handler->Info.EventId.Id);
                                 }
                             }

                             var agentProxy = ShopEventHandler.AgentProxy.Instance();
                             if (agentProxy != null && agentProxy->Handler != null)
                             {
                                 if (agentProxy->Handler == eventHandler.Item2.Value)
                                 {
                                     shopId = shops.FirstOrDefault(c => c.Item2 == eventHandler.Item1);
                                 }
                             }
                         }
                    }
                }
            }

            if (npcId != null)
            {
                var npcRow = this.enpcBaseSheet.GetRowOrDefault(npcId.Value);
                if (npcRow != null)
                {
                    var correctOrder = npcRow.Base.ENpcData.Select(c => c.RowId);
                    shops = shops.OrderBySequence(correctOrder, tuple => tuple.Item2).ToList();
                }

                return (npcId.Value, shops, shopId);
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.addonLifecycle.UnregisterListener(AddonEvent.PostSetup, ["Shop", "FreeCompanyCreditShop", "ShopExchangeItem", "ShopExchangeCurrency", "InclusionShop", "CollectablesShop"], this.AddonPostSetup);
    }

    private void AddonPostSetup(AddonEvent type, AddonArgs args)
    {
        this.OnShopChanged?.Invoke();
        this.OnShopOpened?.Invoke();
    }

    private void AddonPreFinalize(AddonEvent type, AddonArgs args)
    {
        this.OnShopChanged?.Invoke();
        this.OnShopClosed?.Invoke();
    }
}