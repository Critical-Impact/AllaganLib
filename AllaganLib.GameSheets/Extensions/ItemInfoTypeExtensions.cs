using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;

namespace AllaganLib.GameSheets.Extensions;

public static class ItemInfoTypeExtensions
{
    public static bool InCategory(this List<ItemInfoType> itemInfoTypes, ItemInfoCategory category)
    {
        return itemInfoTypes.AsEnumerable().InCategory(category);
    }

    public static bool InCategory(this ItemInfoType itemInfoType, ItemInfoCategory category)
    {
        switch (category)
        {
            case ItemInfoCategory.Gathering:
                return itemInfoType.IsGathering();
            case ItemInfoCategory.RegularGathering:
                return itemInfoType.IsRegularGathering();
            case ItemInfoCategory.EphemeralGathering:
                return itemInfoType.IsEphemeralGathering();
            case ItemInfoCategory.HiddenGathering:
                return itemInfoType.IsHiddenGathering();
            case ItemInfoCategory.TimedGathering:
                return itemInfoType.IsTimedGathering();
            case ItemInfoCategory.NormalVenture:
                return itemInfoType.IsNormalVenture();
            case ItemInfoCategory.ExplorationVenture:
                return itemInfoType.IsExplorationVenture();
            case ItemInfoCategory.AllVentures:
                return itemInfoType.IsExplorationVenture() || itemInfoType.IsNormalVenture() || itemInfoType == ItemInfoType.QuickVenture;
            case ItemInfoCategory.Shop:
                return itemInfoType.IsShop();
        }

        return false;
    }

    public static bool InCategory(this IEnumerable<ItemInfoType> itemInfoTypes, ItemInfoCategory category)
    {
        switch (category)
        {
            case ItemInfoCategory.Gathering:
                return itemInfoTypes.Any(c => c.IsGathering());
            case ItemInfoCategory.RegularGathering:
                return itemInfoTypes.Any(c => c.IsRegularGathering());
            case ItemInfoCategory.EphemeralGathering:
                return itemInfoTypes.Any(c => c.IsEphemeralGathering());
            case ItemInfoCategory.HiddenGathering:
                return itemInfoTypes.Any(c => c.IsHiddenGathering());
            case ItemInfoCategory.TimedGathering:
                return itemInfoTypes.Any(c => c.IsTimedGathering());
            case ItemInfoCategory.NormalVenture:
                return itemInfoTypes.Any(c => c.IsNormalVenture());
            case ItemInfoCategory.ExplorationVenture:
                return itemInfoTypes.Any(c => c.IsExplorationVenture());
            case ItemInfoCategory.AllVentures:
                return itemInfoTypes.Any(c => c.IsExplorationVenture() || c.IsNormalVenture() || c == ItemInfoType.QuickVenture);
            case ItemInfoCategory.Shop:
                return itemInfoTypes.Any(c => c.IsShop());
        }

        return false;
    }
    public static bool IsGathering(this ItemInfoType itemInfoType)
    {
        return itemInfoType is ItemInfoType.Mining
            or ItemInfoType.Quarrying
            or ItemInfoType.Logging
            or ItemInfoType.Harvesting
            or ItemInfoType.HiddenMining
            or ItemInfoType.HiddenQuarrying
            or ItemInfoType.HiddenLogging
            or ItemInfoType.HiddenHarvesting
            or ItemInfoType.TimedMining
            or ItemInfoType.TimedQuarrying
            or ItemInfoType.TimedLogging
            or ItemInfoType.TimedHarvesting
            or ItemInfoType.EphemeralMining
            or ItemInfoType.EphemeralQuarrying
            or ItemInfoType.EphemeralLogging
            or ItemInfoType.EphemeralHarvesting;
    }

    public static bool IsRegularGathering(this ItemInfoType itemInfoType)
    {
        return itemInfoType is ItemInfoType.Mining
            or ItemInfoType.Quarrying
            or ItemInfoType.Logging
            or ItemInfoType.Harvesting;
    }

    public static bool IsHiddenGathering(this ItemInfoType itemInfoType)
    {
        return itemInfoType is ItemInfoType.HiddenMining
            or ItemInfoType.HiddenQuarrying
            or ItemInfoType.HiddenLogging
            or ItemInfoType.HiddenHarvesting;
    }

    public static bool IsEphemeralGathering(this ItemInfoType itemInfoType)
    {
        return itemInfoType is ItemInfoType.TimedHarvesting
            or ItemInfoType.EphemeralMining
            or ItemInfoType.EphemeralQuarrying
            or ItemInfoType.EphemeralLogging
            or ItemInfoType.EphemeralHarvesting;
    }

    public static bool IsTimedGathering(this ItemInfoType itemInfoType)
    {
        return itemInfoType is ItemInfoType.TimedMining
            or ItemInfoType.TimedQuarrying
            or ItemInfoType.TimedLogging
            or ItemInfoType.TimedHarvesting;
    }

    public static bool IsNormalVenture(this ItemInfoType itemInfoType)
    {
        return itemInfoType is ItemInfoType.BotanyVenture
            or ItemInfoType.CombatVenture
            or ItemInfoType.FishingVenture
            or ItemInfoType.MiningVenture;
    }

    public static bool IsExplorationVenture(this ItemInfoType itemInfoType)
    {
        return itemInfoType is ItemInfoType.BotanyExplorationVenture
            or ItemInfoType.CombatExplorationVenture
            or ItemInfoType.FishingExplorationVenture
            or ItemInfoType.MiningExplorationVenture;
    }

    public static bool IsShop(this ItemInfoType itemInfoType)
    {
        return itemInfoType is ItemInfoType.SpecialShop
            or ItemInfoType.FateShop
            or ItemInfoType.GilShop
            or ItemInfoType.FCShop
            or ItemInfoType.GCShop
            or ItemInfoType.CollectablesShop
            or ItemInfoType.CalamitySalvagerShop
            or ItemInfoType.AnimaShop;
    }

    public static bool IsDungeon(this ItemInfoType itemInfoType)
    {
        return itemInfoType is ItemInfoType.DungeonChest
            or ItemInfoType.DungeonDrop
            or ItemInfoType.DungeonBossChest
            or ItemInfoType.DungeonBossDrop;
    }
}