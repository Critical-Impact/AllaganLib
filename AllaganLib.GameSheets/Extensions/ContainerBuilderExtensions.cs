using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Autofac;
using Lumina;
using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.Excel.Services;

namespace AllaganLib.GameSheets.Extensions;

public static class ContainerBuilderExtensions
{
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
        try
        {
            var lines = CsvLoader.LoadResource<T>(
                resourceName,
                out var failedLines,
                gameData,
                gameData.Options.DefaultExcelLanguage);

            if (failedLines.Count != 0)
            {
                foreach (var failedLine in failedLines)
                {
                    // Handle or log failed lines here
                }
            }

            return lines;
        }
        catch (Exception e)
        {
            // Handle or log exception here
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