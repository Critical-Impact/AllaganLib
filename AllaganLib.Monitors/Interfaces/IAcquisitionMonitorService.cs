using System;
using AllaganLib.Monitors.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using Microsoft.Extensions.Hosting;

namespace AllaganLib.Monitors.Interfaces;

/// <summary>
/// Tracks when a new item shows up in a character's primary inventory.
/// </summary>
public interface IAcquisitionMonitorService
{
    /// <summary>
    /// Represents a method that handles when a new item is acquired.
    /// </summary>
    public delegate void ItemAcquiredDelegate(uint itemId, InventoryItem.ItemFlags itemFlags, int qtyIncrease, AcquisitionReason reason);

    /// <summary>
    /// Occurs when a new item is acquired.
    /// </summary>
    public event ItemAcquiredDelegate? ItemAcquired;

    /// <summary>
    /// The configuration of the acquisition tracker,.
    /// </summary>
    public IAcquisitionMonitorConfiguration Configuration { get; }
}