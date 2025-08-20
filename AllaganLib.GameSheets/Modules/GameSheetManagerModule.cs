using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Autofac;
using Lumina;
using Lumina.Excel;
using LuminaSupplemental.Excel.Model;
using Module = Autofac.Module;

namespace AllaganLib.GameSheets.Modules;

public class GameSheetManagerModule : Module
{
    /// <summary>
    /// Gets or sets the default startup options for the sheet manager.
    /// </summary>
    public SheetManagerStartupOptions? StartupOptions { get; set; }

    protected override void Load(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<SheetManager>().SingleInstance();
        containerBuilder.Register<SheetManagerStartupOptions>(c => this.StartupOptions ?? new SheetManagerStartupOptions()).SingleInstance();
        containerBuilder.Register<SheetIndexer>(c => c.Resolve<SheetManager>().SheetIndexer).SingleInstance().ExternallyOwned();
        containerBuilder.Register<ItemInfoCache>(c => c.Resolve<SheetManager>().ItemInfoCache).SingleInstance().ExternallyOwned();
        containerBuilder.Register<NpcLevelCache>(c => c.Resolve<SheetManager>().NpcLevelCache).SingleInstance().ExternallyOwned();
        containerBuilder.Register<NpcShopCache>(c => c.Resolve<SheetManager>().NpcShopCache).SingleInstance().ExternallyOwned();
        containerBuilder.RegisterModule<GameDataModule>();

        Assembly assembly = typeof(SheetManager).Assembly;

        var extendedSheetTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && (t.IsExtendedSheet() || t.IsExtendedSubrowSheet()));

        foreach (var extendedSheetType in extendedSheetTypes)
        {
            containerBuilder.Register(context =>
            {
                return context.Resolve<SheetManager>().SheetContainer.Resolve(extendedSheetType);
            }).As(extendedSheetType).SingleInstance().ExternallyOwned();
        }

        var extendedSubrowSheets = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsExtendedSubrowSheet());

        foreach (var sheetType in extendedSubrowSheets)
        {
            var baseInterface = sheetType.BaseType;
            if (baseInterface != null)
            {
                containerBuilder.RegisterType(sheetType).As(baseInterface).AsSelf().As<IExtendedSheet>().SingleInstance();
            }
        }

        Assembly luminaSupplemental = typeof(ICsv).Assembly;

        var csvs = luminaSupplemental.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterface("ICsv") != null);

        foreach (var csvType in csvs)
        {
            var listType = typeof(List<>).MakeGenericType(csvType);
            containerBuilder.Register(context =>
            {
                return context.Resolve<SheetManager>().SheetContainer.Resolve(listType);
            }).As(listType).SingleInstance().ExternallyOwned();
        }
    }
}