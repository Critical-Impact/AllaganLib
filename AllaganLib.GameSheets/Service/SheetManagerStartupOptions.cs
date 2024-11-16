using System;
using Autofac;

namespace AllaganLib.GameSheets.Service;

public class SheetManagerStartupOptions
{
    /// <summary>
    /// Build the NPC level cache, this is required to use the NpcLevelCache.
    /// </summary>
    public bool BuildNpcLevels { get; set; } = true;

    /// <summary>
    /// Build the NPC shop cache, this is required to use the NpcShopCache.
    /// </summary>
    public bool BuildNpcShops { get; set; } = true;

    /// <summary>
    /// Build the item info cache. You can still use the ItemInfoCache, but you'll need to manually call BuildCache.
    /// </summary>
    public bool BuildItemInfoCache { get; set; } = true;

    /// <summary>
    /// Precalculate lookups in sheets. You can set this to false but you'll need to decide when to calculate lookups. If this is set to false, you'll need to turn off the other options as they will rely on the Lookups.
    /// </summary>
    public bool CalculateLookups { get; set; } = true;

    /// <summary>
    /// If you need to add custom sheets/classes to the SheetManager container, you can provide a hook that the SheetManager will run will building it's container.
    /// </summary>
    public Action<ContainerBuilder>? ContainerBuilderHook { get; set; } = null;
}