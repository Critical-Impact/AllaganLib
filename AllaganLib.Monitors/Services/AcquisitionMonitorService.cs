using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AllaganLib.Monitors.Enums;
using AllaganLib.Monitors.Interfaces;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Inventory;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AllaganLib.Monitors.Services;

/// <inheritdoc cref="IAcquisitionMonitorService" />
public class AcquisitionMonitorService : IAcquisitionMonitorService, IHostedService, IDisposable
{
    private readonly IAcquisitionMonitorConfiguration acquisitionMonitorConfiguration;
    private readonly IGameInventory gameInventory;
    private readonly IClientState clientState;
    private readonly ICondition condition;
    private readonly IShopMonitorService shopMonitorService;
    private readonly IGameGui gameGui;
    private readonly IFramework framework;
    private readonly ILogger<AcquisitionMonitorService> logger;
    private readonly IGameInteropProvider gameInteropProvider;
    private Dictionary<(uint, InventoryItem.ItemFlags), long> itemCounts = new();
    private AcquisitionReason currentState = AcquisitionReason.Other;
    private DateTime? stateChangeTime;
    private bool initialCheckPerformed;
    private bool pluginBootCheckPerformed;
    private DateTime? lastLoginTime;
    private Hook<RaptureAtkModuleUpdateDelegate>? raptureAtkModuleUpdateHook;

    /// <summary>
    /// Initializes a new instance of the <see cref="AcquisitionMonitorService"/> class.
    /// </summary>
    /// <param name="acquisitionMonitorConfiguration">The configuration for the monitor.</param>
    /// <param name="gameInventory">Dalamud's game inventory service.</param>
    /// <param name="clientState">Dalamud's client state service.</param>
    /// <param name="condition">Dalamud's condition service.</param>
    /// <param name="shopMonitorService">AllaganLib.Monitor's shop monitor service.</param>
    /// <param name="gameGui">Dalamud's game gui service.</param>
    /// <param name="framework">Dalamud's framework service.</param>
    /// <param name="logger">A logger.</param>
    /// <param name="gameInteropProvider">Dalamud's game interop provider.</param>
    public AcquisitionMonitorService(
        IAcquisitionMonitorConfiguration acquisitionMonitorConfiguration,
        IGameInventory gameInventory,
        IClientState clientState,
        ICondition condition,
        IShopMonitorService shopMonitorService,
        IGameGui gameGui,
        IFramework framework,
        ILogger<AcquisitionMonitorService> logger,
        IGameInteropProvider gameInteropProvider)
    {
        this.acquisitionMonitorConfiguration = acquisitionMonitorConfiguration;
        this.gameInventory = gameInventory;
        this.clientState = clientState;
        this.condition = condition;
        this.shopMonitorService = shopMonitorService;
        this.gameGui = gameGui;
        this.framework = framework;
        this.logger = logger;
        this.gameInteropProvider = gameInteropProvider;
    }

    private unsafe delegate void RaptureAtkModuleUpdateDelegate(RaptureAtkModule* ram, float f1);

    /// <inheritdoc/>
    public event IAcquisitionMonitorService.ItemAcquiredDelegate? ItemAcquired;

    /// <inheritdoc/>
    public IAcquisitionMonitorConfiguration Configuration => this.acquisitionMonitorConfiguration;

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.clientState.Login += this.ClientLoggedIn;
        this.clientState.Logout += this.ClientStateOnLogout;
        this.framework.Update += this.FrameworkOnUpdate;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.raptureAtkModuleUpdateHook?.Disable();
        this.clientState.Login -= this.ClientLoggedIn;
        this.clientState.Logout -= this.ClientStateOnLogout;
        this.framework.Update -= this.FrameworkOnUpdate;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.raptureAtkModuleUpdateHook?.Dispose();
    }

    private unsafe void RaptureAtkModuleUpdateDetour(RaptureAtkModule* ram, float f1)
    {
        var agentUpdateFlag = ram->AgentUpdateFlag;
        if (agentUpdateFlag != 0)
        {
            var actualFlags = Enum.GetValues(agentUpdateFlag.GetType()).Cast<Enum>().Where(agentUpdateFlag.HasFlag).Select(c => c.ToString());
            this.logger.LogTrace("Agent update flag is {AgentUpdateFlag}, types are {Types}", (int)agentUpdateFlag, actualFlags);
            try
            {
                if (this.initialCheckPerformed && this.pluginBootCheckPerformed && agentUpdateFlag.HasFlag(RaptureAtkModule.AgentUpdateFlags.InventoryUpdate))
                {
                    this.CalculateItemCounts();
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed in RaptureAtkModuleUpdateDetour");
            }
        }

        this.raptureAtkModuleUpdateHook!.Original(ram, f1);
    }

    private void CalculateItemCounts(bool notify = true)
    {
        if (!this.clientState.IsLoggedIn)
        {
            return;
        }

        this.logger.LogTrace("Calculating item counts, set to notify: {Notify}", notify ? "true" : "false");
        GameInventoryType[] inventories =
        [
            GameInventoryType.Inventory1, GameInventoryType.Inventory2, GameInventoryType.Inventory3,
            GameInventoryType.Inventory4, GameInventoryType.Currency, GameInventoryType.Crystals
        ];

        var newItemCounts = new Dictionary<(uint, InventoryItem.ItemFlags), long>();
        foreach (var inventory in inventories)
        {
            foreach (var item in this.gameInventory.GetInventoryItems(inventory))
            {
                var itemType = InventoryItem.ItemFlags.None;
                if (item.IsHq)
                {
                    itemType = InventoryItem.ItemFlags.HighQuality;
                }
                else if (item.IsCollectable)
                {
                    itemType = InventoryItem.ItemFlags.Collectable;
                }

                newItemCounts.TryAdd((item.BaseItemId, itemType), 0);
                newItemCounts[(item.BaseItemId, itemType)] += item.Quantity;
            }
        }

        if (notify)
        {
            var changed = new Dictionary<(uint, InventoryItem.ItemFlags), long>();

            foreach (var itemCount in newItemCounts)
            {
                if (this.itemCounts.TryGetValue(itemCount.Key, out var oldCount))
                {
                    var newCount = itemCount.Value;
                    if (newCount > oldCount)
                    {
                        changed.Add(itemCount.Key, newCount - oldCount);
                    }
                }
                else
                {
                    changed.Add(itemCount.Key, itemCount.Value);
                }
            }

            foreach (var itemChange in changed)
            {
                this.logger.LogTrace("Item change detected, {ItemId} of {ItemType} quality increased by {Quantity}", itemChange.Key.Item1, itemChange.Key.Item2, itemChange.Value);
                this.ItemAcquired?.Invoke(itemChange.Key.Item1, itemChange.Key.Item2, (int)itemChange.Value, this.currentState);
            }
        }

        this.itemCounts = newItemCounts;
    }

    private void FrameworkOnUpdate(IFramework iFramework)
    {
        if (this.raptureAtkModuleUpdateHook == null)
        {
            unsafe
            {
                this.raptureAtkModuleUpdateHook = this.gameInteropProvider.HookFromFunctionPointerVariable<RaptureAtkModuleUpdateDelegate>(
                    new(&RaptureAtkModule.StaticVirtualTablePointer->Update),
                    this.RaptureAtkModuleUpdateDetour);
            }

            this.raptureAtkModuleUpdateHook.Enable();
        }

        // When the plugin first loads, check to see if they are already logged in, if so then their inventory is probably already loaded so we can scan once and mark it as checked
        // If they are not logged in we'll need to wait for them to login and then scan
        if (!this.pluginBootCheckPerformed)
        {
            this.logger.LogTrace("Performing initial tracker service login check");
            this.pluginBootCheckPerformed = true;
            if (this.clientState.IsLoggedIn)
            {
                this.logger.LogTrace("Character already logged in, checking item counts.");
                this.CalculateItemCounts(false);
                this.initialCheckPerformed = true;
            }
            else
            {
                this.logger.LogTrace("Character not logged in, waiting until login to start.");
            }
        }

        if (!this.clientState.IsLoggedIn)
        {
            return;
        }

        if (!this.initialCheckPerformed)
        {
            if (this.lastLoginTime == null)
            {
                this.logger.LogTrace("Character logged in but no login time detected, setting to now.");
                this.lastLoginTime = DateTime.Now;
            }

            var loginDelay = this.acquisitionMonitorConfiguration.LoginDelay;
            if (this.lastLoginTime.Value.AddSeconds(loginDelay) <= DateTime.Now)
            {
                this.logger.LogTrace("{LoginDelay} seconds has elapsed since login, generating item counts.", loginDelay);
                this.lastLoginTime = null;
                this.initialCheckPerformed = true;
                this.CalculateItemCounts(false);
            }
            else
            {
                return;
            }
        }

        var acquisitionReason = AcquisitionReason.Other;

        if (this.condition[ConditionFlag.Crafting] || this.condition[ConditionFlag.ExecutingCraftingAction])
        {
            acquisitionReason = AcquisitionReason.Crafting;
        }
        else if (this.condition[ConditionFlag.Gathering] || this.condition[ConditionFlag.ExecutingGatheringAction])
        {
            acquisitionReason = AcquisitionReason.Gathering;
        }
        else if (this.shopMonitorService.GetCurrentShopType() != null)
        {
            acquisitionReason = AcquisitionReason.Shopping;
        }
        else if (this.condition[ConditionFlag.InCombat])
        {
            acquisitionReason = AcquisitionReason.CombatDrop;
        }
        else if (this.gameGui.GetAddonByName("ItemSearch").IsVisible)
        {
            acquisitionReason = AcquisitionReason.Marketboard;
        }

        if (this.currentState != acquisitionReason)
        {
            if (this.stateChangeTime == null)
            {
                if (this.currentState == AcquisitionReason.Other)
                {
                    this.logger.LogTrace("Currently in {CurrentState}, moving to {NewState}", this.currentState, acquisitionReason);
                    this.currentState = acquisitionReason;
                }
                else
                {
                    var persistStateSecond = this.acquisitionMonitorConfiguration.PersistStateTime;
                    this.logger.LogTrace("Currently in {CurrentState}, moving to {NewState} in {Time} seconds", this.currentState, acquisitionReason, persistStateSecond);
                    this.stateChangeTime = DateTime.Now + TimeSpan.FromSeconds(persistStateSecond);
                }
            }
            else if (DateTime.Now > this.stateChangeTime)
            {
                this.stateChangeTime = null;
                this.currentState = acquisitionReason;
                this.logger.LogTrace("State changed to {NewState}", this.currentState);
            }
        }
    }

    private void ClientStateOnLogout(int type, int code)
    {
        this.logger.LogTrace("Character has logged out, clearing items, setting initial check and removing last login time.");
        this.itemCounts.Clear();
        this.initialCheckPerformed = false;
        this.lastLoginTime = null;
    }

    private void ClientLoggedIn()
    {
        this.logger.LogTrace("Character has logged in, setting initial check and setting last login time..");
        this.initialCheckPerformed = false;
        this.lastLoginTime = DateTime.Now;
    }
}