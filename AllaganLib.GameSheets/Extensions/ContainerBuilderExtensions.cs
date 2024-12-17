using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Autofac;
using Lumina;
using Lumina.Excel;
using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.Excel.Services;
using ILogger = Serilog.ILogger;

namespace AllaganLib.GameSheets.Extensions;

public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Registers the services/classes required to use SheetManager in your own container. This starts a second container that the SheetManager controls, allowing it to handle it's own disposal.
    /// </summary>
    /// <param name="containerBuilder"></param>
    /// <param name="sheetManagerStartupOptions"></param>
    public static void RegisterGameSheetManager(this ContainerBuilder containerBuilder, SheetManagerStartupOptions? sheetManagerStartupOptions = null)
    {
        containerBuilder.RegisterType<SheetManager>().SingleInstance();
        containerBuilder.Register<SheetManagerStartupOptions>(c => sheetManagerStartupOptions ?? new SheetManagerStartupOptions()).SingleInstance();
        containerBuilder.Register<SheetIndexer>(c => c.Resolve<SheetManager>().SheetIndexer).SingleInstance().ExternallyOwned();
        containerBuilder.Register<ItemInfoCache>(c => c.Resolve<SheetManager>().ItemInfoCache).SingleInstance().ExternallyOwned();
        containerBuilder.Register<NpcLevelCache>(c => c.Resolve<SheetManager>().NpcLevelCache).SingleInstance().ExternallyOwned();
        containerBuilder.Register<NpcShopCache>(c => c.Resolve<SheetManager>().NpcShopCache).SingleInstance().ExternallyOwned();
        containerBuilder.RegisterGeneric((context, parameters) =>
        {
            var gameData = context.Resolve<GameData>();
            var method = typeof(GameData).GetMethod(nameof(GameData.GetExcelSheet))
                ?.MakeGenericMethod(parameters);
            var sheet = method!.Invoke(gameData, [null, null])!;
            return sheet;
        })
        .As(typeof(ExcelSheet<>));
        containerBuilder.RegisterGeneric((context, parameters) =>
        {
            var gameData = context.Resolve<GameData>();
            var method = typeof(GameData).GetMethod(nameof(GameData.GetSubrowExcelSheet))
                ?.MakeGenericMethod(parameters);
            var sheet = method!.Invoke(gameData, [null, null])!;
            return sheet;
        })
        .As(typeof(SubrowExcelSheet<>));

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

    public static void RegisterExtendedSheets(this ContainerBuilder builder)
    {
        Assembly assembly = typeof(SheetManager).Assembly;

        var extendedSheetTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && IsExtendedSheet(t));

        foreach (var sheetType in extendedSheetTypes)
        {
            var baseInterface = sheetType.BaseType;
            if (baseInterface != null)
            {
                builder.RegisterType(sheetType).As(baseInterface).AsSelf().As<IExtendedSheet>().SingleInstance();
            }
        }

        var extendedSubrowSheets = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && IsExtendedSubrowSheet(t));

        foreach (var sheetType in extendedSubrowSheets)
        {
            var baseInterface = sheetType.BaseType;
            if (baseInterface != null)
            {
                builder.RegisterType(sheetType).As(baseInterface).AsSelf().As<IExtendedSheet>().SingleInstance();
            }
        }
    }

    public static void RegisterCsv<T>(this ContainerBuilder builder, string resourceName)
        where T : ICsv, new()
    {
        builder.Register(context => LoadCsvData<T>(context, resourceName))
            .As<List<T>>()
            .SingleInstance();
    }

    private static List<T> LoadCsvData<T>(IComponentContext context, string resourceName)
        where T : ICsv, new()
    {
        var gameData = context.Resolve<GameData>();
        ILogger? logger = null;
        context.TryResolve(out logger);
        logger?.Verbose($"Loading data from {resourceName}");
        try
        {
            var lines = CsvLoader.LoadResource<T>(
                resourceName,
                out var failedLines,
                out var exceptions,
                gameData,
                gameData.Options.DefaultExcelLanguage);

            foreach (var exception in exceptions.Select(c => c.Message))
            {
                logger?.Error(string.Join(",", exception));
            }

            if (failedLines.Count != 0)
            {
                logger?.Error($"Failed to load CSV data from {resourceName}: {string.Join(",", failedLines)}");
            }

            return lines;
        }
        catch (Exception e)
        {
            logger?.Error($"Failed to load CSV data from {resourceName} - " + e.ToString());
        }

        return new List<T>();
    }

    public static bool IsExtendedSheet(this Type type)
    {
        if (type.BaseType is not { IsGenericType: true })
        {
            return false;
        }

        var baseGenericType = type.BaseType.GetGenericTypeDefinition();
        return baseGenericType == typeof(ExtendedSheet<,,>);
    }

    public static bool IsExtendedSubrowSheet(this Type type)
    {
        if (type.BaseType is not { IsGenericType: true })
        {
            return false;
        }

        var baseGenericType = type.BaseType.GetGenericTypeDefinition();
        return baseGenericType == typeof(ExtendedSubrowSheet<,,>);
    }
}