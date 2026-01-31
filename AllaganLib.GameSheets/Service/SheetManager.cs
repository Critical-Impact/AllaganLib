using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using Autofac;
using Lumina;
using Lumina.Excel;
using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.Excel.Services;

using ILogger = Serilog.ILogger;

namespace AllaganLib.GameSheets.Service;

/// <summary>
/// SheetManager creates an extra layer on top of Lumina, allowing you to cache extra data on custom Sheets and Rows.
/// </summary>
public class SheetManager : IDisposable, IAsyncDisposable
{
    private IContainer sheetContainer;
    private Dictionary<Type, object> sheetCache;
    private SheetIndexer? sheetIndexer;
    private NpcShopCache? npcShopCache;
    private NpcLevelCache? npcLevelCache;
    private ItemInfoCache? itemInfoCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="SheetManager"/> class.
    /// </summary>
    /// <param name="gameData">An instance of Lumina's GameData. If you are using dalamud, this is available via IDataManager.GameData.</param>
    /// <param name="startupOptions">The options to use when constructing the sheet manager.</param>
    public SheetManager(GameData gameData, SheetManagerStartupOptions startupOptions)
    {
        this.sheetContainer = this.CreateContainer(gameData, startupOptions);
        this.sheetCache = new Dictionary<Type, object>();
        if (startupOptions.BuildNpcLevels)
        {
            this.SheetContainer.Resolve<NpcLevelCache>().BuildLevelMap();
        }

        if (startupOptions.BuildNpcShops)
        {
            this.SheetContainer.Resolve<NpcShopCache>().BuildDataMap();
        }

        if (startupOptions.CalculateLookups)
        {
            var sheets = this.SheetContainer.Resolve<IEnumerable<IExtendedSheet>>();
            foreach (var sheet in sheets)
            {
                sheet.CalculateLookups();
            }
        }

        if (startupOptions.BuildItemInfoCache)
        {
            this.SheetContainer.Resolve<ItemInfoCache>().BuildCache();
        }
    }

    public IContainer SheetContainer => this.sheetContainer;

    public SheetIndexer SheetIndexer => this.sheetIndexer ??= this.sheetContainer.Resolve<SheetIndexer>();

    public NpcShopCache NpcShopCache => this.npcShopCache ??= this.sheetContainer.Resolve<NpcShopCache>();

    public NpcLevelCache NpcLevelCache => this.npcLevelCache ??= this.sheetContainer.Resolve<NpcLevelCache>();

    public ItemInfoCache ItemInfoCache => this.itemInfoCache ??= this.sheetContainer.Resolve<ItemInfoCache>();

    private IContainer CreateContainer(GameData gameData, SheetManagerStartupOptions startupOptions)
    {
        var containerBuilder = new ContainerBuilder();
        containerBuilder.RegisterInstance(gameData);
        containerBuilder.RegisterType<SheetIndexer>().SingleInstance();
        containerBuilder.RegisterInstance(this).SingleInstance();
        containerBuilder.RegisterType<NpcShopCache>().SingleInstance();
        containerBuilder.RegisterType<NpcLevelCache>().SingleInstance();
        containerBuilder.RegisterType<ItemInfoCache>().SingleInstance();
        if (startupOptions.Logger != null)
        {
            containerBuilder.RegisterInstance(startupOptions.Logger).As<ILogger>();
        }

        containerBuilder.RegisterInstance(startupOptions).SingleInstance();
        containerBuilder.RegisterExtendedSheets();
        containerBuilder.RegisterCsv<AirshipDrop>(CsvLoader.AirshipDropResourceName);
        containerBuilder.RegisterCsv<AirshipUnlock>(CsvLoader.AirshipUnlockResourceName);
        containerBuilder.RegisterCsv<MobSpawnPosition>(CsvLoader.MobSpawnResourceName);
        containerBuilder.RegisterCsv<MobDrop>(CsvLoader.MobDropResourceName);
        containerBuilder.RegisterCsv<ENpcPlace>(CsvLoader.ENpcPlaceResourceName);
        containerBuilder.RegisterCsv<ENpcShop>(CsvLoader.ENpcShopResourceName);
        containerBuilder.RegisterCsv<HouseVendor>(CsvLoader.HouseVendorResourceName);
        containerBuilder.RegisterCsv<ShopName>(CsvLoader.ShopNameResourceName);
        containerBuilder.RegisterCsv<RetainerVentureItem>(CsvLoader.RetainerVentureItemResourceName);
        containerBuilder.RegisterCsv<SubmarineDrop>(CsvLoader.SubmarineDropResourceName);
        containerBuilder.RegisterCsv<SubmarineUnlock>(CsvLoader.SubmarineUnlockResourceName);
        containerBuilder.RegisterCsv<StoreItem>(CsvLoader.StoreItemResourceName);
        containerBuilder.RegisterCsv<DungeonBoss>(CsvLoader.DungeonBossResourceName);
        containerBuilder.RegisterCsv<DungeonChest>(CsvLoader.DungeonChestResourceName);
        containerBuilder.RegisterCsv<Gearset>(CsvLoader.GearsetResourceName);
        containerBuilder.RegisterCsv<DungeonChestItem>(CsvLoader.DungeonChestItemResourceName);
        containerBuilder.RegisterCsv<DungeonDrop>(CsvLoader.DungeonDropItemResourceName);
        containerBuilder.RegisterCsv<DungeonBossDrop>(CsvLoader.DungeonBossDropResourceName);
        containerBuilder.RegisterCsv<DungeonBossChest>(CsvLoader.DungeonBossChestResourceName);
        containerBuilder.RegisterCsv<FateItem>(CsvLoader.FateItemResourceName);
        containerBuilder.RegisterCsv<ItemPatch>(CsvLoader.ItemPatchResourceName);
        containerBuilder.RegisterCsv<ItemSupplement>(CsvLoader.ItemSupplementResourceName);
        containerBuilder.RegisterCsv<GardeningCrossbreed>(CsvLoader.GardeningCrossbreedResourceName);
        containerBuilder.RegisterCsv<FestivalName>(CsvLoader.FestivalNameResourceName);
        containerBuilder.RegisterCsv<FieldOpCoffer>(CsvLoader.FieldOpCofferResourceName);
        containerBuilder.RegisterCsv<QuestRequiredItem>(CsvLoader.QuestRequiredItemResourceName);
        if (startupOptions.ContainerBuilderHook != null)
        {
            startupOptions.ContainerBuilderHook.Invoke(containerBuilder);
        }

        return containerBuilder.Build();
    }

    public TExtendedSheet GetSheet<TExtendedSheet>()
        where TExtendedSheet : IExtendedSheet
    {
        Type sheetType = typeof(TExtendedSheet);
        if (!this.sheetCache.TryGetValue(sheetType, out object? value))
        {
            TExtendedSheet sheet = this.SheetContainer.Resolve<TExtendedSheet>();
            value = sheet;
            this.sheetCache[sheetType] = value;
        }

        return (TExtendedSheet)value;
    }

    public TExtendedSheet GetSubrowSheet<TBase, TExtendedRow, TExtendedSheet>()
        where TBase : struct, IExcelSubrow<TBase>
        where TExtendedRow : ExtendedSubrow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSubrowSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
    {
        Type sheetType = typeof(TExtendedSheet);
        if (!this.sheetCache.TryGetValue(sheetType, out object? value))
        {
            TExtendedSheet sheet = this.SheetContainer.Resolve<TExtendedSheet>();
            value = sheet;
            this.sheetCache[sheetType] = value;
        }

        return (TExtendedSheet)value;
    }

    public void Dispose()
    {
        this.sheetCache.Clear();
        this.sheetContainer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await this.sheetContainer.DisposeAsync();
    }
}