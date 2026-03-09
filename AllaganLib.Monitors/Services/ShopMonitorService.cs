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
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EventHandler = FFXIVClientStructs.FFXIV.Client.Game.Event.EventHandler;

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

    //TopicSelect -> (Prehandler, Shop)
    private readonly Dictionary<uint, HashSet<(uint, uint)>> gilShopTopicSelectPrehandler = new();
    private readonly Dictionary<uint, HashSet<(uint, uint)>> specialShopTopicSelectPrehandler = new();
    private readonly Dictionary<uint, HashSet<(uint, uint)>> collectableShopTopicSelectPrehandler = new();
    private readonly Dictionary<uint, HashSet<(uint, uint)>> inclusionShopTopicSelectPrehandler = new();

    private readonly Dictionary<uint, HashSet<uint>> collectableShopSpecialLink = new();
    private readonly ExcelModule module;

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
        this.module = dataManager.Excel;
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
                            this.collectableShopTopicSelectPrehandler.TryAdd(topicSelect.RowId, []);
                            this.collectableShopTopicSelectPrehandler[topicSelect.RowId].Add((preHandler.Value.RowId, preHandler.Value.Target.RowId));
                        }
                        else if (preHandler.Value.Target.Is<Lumina.Excel.Sheets.InclusionShop>())
                        {
                            this.inclusionShopTopicSelectPrehandler.TryAdd(topicSelect.RowId, []);
                            this.inclusionShopTopicSelectPrehandler[topicSelect.RowId].Add((preHandler.Value.RowId, preHandler.Value.Target.RowId));
                        }
                        else if (preHandler.Value.Target.Is<Lumina.Excel.Sheets.GilShop>())
                        {
                            this.gilShopTopicSelectPrehandler.TryAdd(topicSelect.RowId, []);
                            this.gilShopTopicSelectPrehandler[topicSelect.RowId].Add((preHandler.Value.RowId, preHandler.Value.Target.RowId));
                        }
                        else if (preHandler.Value.Target.Is<Lumina.Excel.Sheets.SpecialShop>())
                        {
                            this.specialShopTopicSelectPrehandler.TryAdd(topicSelect.RowId, []);
                            this.specialShopTopicSelectPrehandler[topicSelect.RowId].Add((preHandler.Value.RowId, preHandler.Value.Target.RowId));
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
    public (ENpcBaseRow Npc, List<List<IShop>> Shops, List<List<IShop>>? SubShops, IShop? ActiveShop)? GetCurrentShopType()
    {
        var currentShopIds = this.GetCurrentShopTypeIds();
        if (currentShopIds == null)
        {
            return null;
        }

        var shopIds = currentShopIds.Value.MenuItems;
        var activeShopId = currentShopIds.Value.ActiveShopId;

        List<List<IShop>> groupedShops = [];
        IShop? activeShop = null;

        foreach (var shopMenu in shopIds)
        {
            var shops = new List<IShop>();
            foreach (var shopId in shopMenu.Shops)
            {
                var shop = this.GetShopByIdAndType(shopId.ShopId, shopId.ShopType);
                if (shop != null)
                {
                    shops.Add(shop);
                }
            }

            groupedShops.Add(shops);
        }

        var subShopIds = currentShopIds.Value.SubmenuItems;

        List<List<IShop>>? subGroupedShops = null;

        if (subShopIds != null)
        {
            subGroupedShops = [];
            foreach (var shopMenu in subShopIds)
            {
                var shops = new List<IShop>();
                foreach (var shopId in shopMenu.Shops)
                {
                    var shop = this.GetShopByIdAndType(shopId.ShopId, shopId.ShopType);
                    if (shop != null)
                    {
                        shops.Add(shop);
                    }
                }

                subGroupedShops.Add(shops);
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

        return (currentShopIds.Value.ENpcBase, groupedShops, subGroupedShops, activeShop);
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
    public unsafe (ENpcBaseRow ENpcBase, List<IShopMenu> MenuItems, List<IShopMenu>? SubmenuItems, (ShopType, uint)? ActiveShopId)? GetCurrentShopTypeIds()
    {
        var eventFramework = EventFramework.Instance();
        if (eventFramework != null)
        {
            uint? npcId = null;
            ENpcBaseRow? npcBase = null;
            List<IShopMenu> menuItems = [];
            List<IShopMenu>? submenuItems = null;
            (ShopType, uint)? shopId = null;
            List<StdPair<uint, Pointer<EventHandler>>> relatedItems = [];
            HashSet<uint> activeEventHandlers = [];

            foreach (var eventHandler in eventFramework->EventHandlerModule.EventHandlerMap)
            {
                if (eventHandler.Item2.Value->SceneGameObject != null)
                {
                    activeEventHandlers.Add(eventHandler.Item1);
                }

                var related = false;
                foreach (var eventObject in eventHandler.Item2.Value->EventObjects)
                {
                    if (this.targetManager.Target?.BaseId == eventObject.Value->BaseId)
                    {
                        npcId = this.targetManager.Target?.BaseId;
                        related = true;
                    }
                }

                if (related)
                {
                    relatedItems.Add(eventHandler);
                }
            }

            if (npcId != null)
            {
                npcBase = this.enpcBaseSheet.GetRowOrDefault(npcId.Value);
                if (npcBase != null)
                {
                    var correctOrder = npcBase.Base.ENpcData.Select(c => c.RowId);

                    foreach (var eventHandler in relatedItems.OrderBySequence(correctOrder, tuple => tuple.Item1))
                    {
                        bool isActive = false;
                        if (eventHandler.Item2.Value->SceneGameObject != null)
                        {
                            isActive = true;
                        }

                        if (eventHandler.Item2.Value != null)
                        {
                            if (eventHandler.Item2.Value->Info.EventId.ContentId == EventHandlerContent.Shop)
                            {
                                if (this.collectableShops.Contains(eventHandler.Item1))
                                {
                                    menuItems.Add(new ShopMenu([(ShopType.Collectable, eventHandler.Item1)], isActive));
                                }
                                else if (this.inclusionShops.Contains(eventHandler.Item1))
                                {
                                    menuItems.Add(new ShopMenu([(ShopType.InclusionShop, eventHandler.Item1)], isActive));
                                }
                                else if (this.gilShops.Contains(eventHandler.Item1))
                                {
                                    menuItems.Add(new ShopMenu([(ShopType.Gil, eventHandler.Item1)], isActive));
                                }
                                else if (this.specialShops.Contains(eventHandler.Item1))
                                {
                                    menuItems.Add(new ShopMenu([(ShopType.SpecialShop, eventHandler.Item1)], isActive));
                                }
                                else if (this.fccShops.Contains(eventHandler.Item1))
                                {
                                    menuItems.Add(new ShopMenu([(ShopType.FreeCompanyShop, eventHandler.Item1)], isActive));
                                }
                            }

                            if (eventHandler.Item2.Value->Info.EventId.ContentId == EventHandlerContent.CustomTalk)
                            {
                                if (this.collectableShopCustomTalk.TryGetValue(eventHandler.Item1, out var value))
                                {
                                    foreach (var customTalkShopId in value)
                                    {
                                        menuItems.Add(new ShopMenu([(ShopType.Collectable, customTalkShopId)], isActive));
                                    }
                                }
                                else if (this.inclusionShopCustomTalk.TryGetValue(eventHandler.Item1, out var value1))
                                {
                                    foreach (var customTalkShopId in value1)
                                    {
                                        menuItems.Add(new ShopMenu([(ShopType.InclusionShop, customTalkShopId)], isActive));
                                    }
                                }
                                else if (this.gilShopCustomTalk.TryGetValue(eventHandler.Item1, out var value2))
                                {
                                    foreach (var customTalkShopId in value2)
                                    {
                                        menuItems.Add(new ShopMenu([(ShopType.Gil, customTalkShopId)], isActive));
                                    }
                                }
                                else if (this.specialShopCustomTalk.TryGetValue(eventHandler.Item1, out var value3))
                                {
                                    foreach (var customTalkShopId in value3)
                                    {
                                        menuItems.Add(new ShopMenu([(ShopType.SpecialShop, customTalkShopId)], isActive));
                                    }
                                }
                                else if (this.fccShopCustomTalk.TryGetValue(eventHandler.Item1, out var value4))
                                {
                                    foreach (var customTalkShopId in value4)
                                    {
                                        menuItems.Add(new ShopMenu([(ShopType.FreeCompanyShop, customTalkShopId)], isActive));
                                    }
                                }
                                else if (this.collectableShopSpecialLink.TryGetValue(eventHandler.Item1, out var value9))
                                {
                                    foreach (var collectableShopId in value9)
                                    {
                                        menuItems.Add(new ShopMenu([(ShopType.Collectable, collectableShopId)], isActive));
                                    }
                                }
                            }

                            if (eventHandler.Item2.Value->Info.EventId.ContentId == EventHandlerContent.PreHandler)
                            {
                                if (this.collectableShopPreHandlers.TryGetValue(eventHandler.Item1, out var value1))
                                {
                                    menuItems.Add(new ShopMenu([(ShopType.Collectable, value1)], isActive));
                                }
                                else if (this.inclusionShopPreHandlers.TryGetValue(eventHandler.Item1, out var value2))
                                {
                                    menuItems.Add(new ShopMenu([(ShopType.InclusionShop, value2)], isActive));
                                }
                                else if (this.gilShopPreHandlers.TryGetValue(eventHandler.Item1, out var value3))
                                {
                                    menuItems.Add(new ShopMenu([(ShopType.Gil, value3)], isActive));
                                }
                                else if (this.specialShopPreHandlers.TryGetValue(eventHandler.Item1, out var value4))
                                {
                                    menuItems.Add(new ShopMenu([(ShopType.SpecialShop, value4)], isActive));
                                }
                            }

                            if (eventHandler.Item2.Value->Info.EventId.ContentId == EventHandlerContent.TopicSelect)
                            {
                                if (this.gilShopTopicSelect.TryGetValue(eventHandler.Item1, out var value1))
                                {
                                    menuItems.Add(new ShopMenu(value1.Select(c => (ShopType.Gil, c)).ToList(), isActive, new RowRef<TopicSelect>(this.module, eventHandler.Item1)));
                                    if (activeEventHandlers.Contains(eventHandler.Item1))
                                    {
                                        foreach (var topicSelectShopId in value1)
                                        {
                                            submenuItems ??= [];
                                            submenuItems.Add(new ShopMenu([(ShopType.Gil, topicSelectShopId)], activeEventHandlers.Contains(topicSelectShopId)));
                                        }
                                    }
                                }
                                else if (this.specialShopTopicSelect.TryGetValue(eventHandler.Item1, out var value2))
                                {
                                    menuItems.Add(new ShopMenu(value2.Select(c => (ShopType.SpecialShop, c)).ToList(), isActive, new RowRef<TopicSelect>(this.module, eventHandler.Item1)));
                                    if (activeEventHandlers.Contains(eventHandler.Item1))
                                    {
                                        foreach (var topicSelectShopId in value2)
                                        {
                                            submenuItems ??= [];
                                            submenuItems.Add(new ShopMenu([(ShopType.SpecialShop, topicSelectShopId)], activeEventHandlers.Contains(topicSelectShopId)));
                                        }
                                    }
                                }
                                //handle this
                                else if (this.gilShopTopicSelectPrehandler.TryGetValue(eventHandler.Item1, out var value3))
                                {
                                    menuItems.Add(new ShopMenu([], isActive, new RowRef<TopicSelect>(this.module, eventHandler.Item1)));
                                    if (activeEventHandlers.Contains(eventHandler.Item1))
                                    {
                                        foreach (var topicSelectShopId in value3)
                                        {
                                            submenuItems ??= [];
                                            submenuItems.Add(new ShopMenu([(ShopType.Gil, topicSelectShopId.Item2)], activeEventHandlers.Contains(topicSelectShopId.Item2)));
                                        }
                                    }
                                }
                                else if (this.specialShopTopicSelectPrehandler.TryGetValue(eventHandler.Item1, out var value4))
                                {
                                    menuItems.Add(new ShopMenu([], isActive, new RowRef<TopicSelect>(this.module, eventHandler.Item1)));
                                    if (activeEventHandlers.Contains(eventHandler.Item1))
                                    {
                                        foreach (var topicSelectShopId in value4)
                                        {
                                            submenuItems ??= [];
                                            submenuItems.Add(new ShopMenu([(ShopType.SpecialShop, topicSelectShopId.Item2)], activeEventHandlers.Contains(topicSelectShopId.Item2)));
                                        }
                                    }
                                }
                            }

                            if (eventHandler.Item2.Value->EventSceneModule != null && eventHandler.Item2.Value->Info.EventId.ContentId == EventHandlerContent.Shop)
                            {
                                if (menuItems.Count > 0 && menuItems[0].Shops.Count > 0)
                                {
                                    shopId = menuItems[0].Shops.Last();
                                }
                            }
                        }
                    }
                    return (npcBase, menuItems, submenuItems, shopId);
                }
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