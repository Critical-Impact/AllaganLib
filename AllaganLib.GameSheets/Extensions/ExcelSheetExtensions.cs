using System;
using System.Collections.Generic;
using Lumina.Data;
using Lumina.Excel;

namespace AllaganLib.GameSheets.Extensions;

public static class ExcelSheetExtensions
{
    /// <summary>
    /// Converts a IExcelRow row into a typed RowRef.
    /// </summary>
    /// <param name="row">The IExcelRow.</param>
    /// <param name="language">A different language or the same language as the original.</param>
    /// <typeparam name="T">A lumina sheet.</typeparam>
    /// <returns>A typed RowRef.</returns>
    public static RowRef<T> AsTypedRowRef<T>(this IExcelRow<T> row, Language? language = null)
        where T : struct, IExcelRow<T>
    {
        return new RowRef<T>(row.ExcelPage.Module, row.RowId, language ?? row.ExcelPage.Language);
    }

    /// <summary>
    /// Converts a IExcelRow row into a untyped RowRef.
    /// </summary>
    /// <param name="row">The IExcelRow.</param>
    /// <param name="language">A different language or the same language as the original.</param>
    /// <typeparam name="T">A lumina sheet.</typeparam>
    /// <returns>A untyped RowRef.</returns>
    public static RowRef AsUntypedRowRef<T>(this IExcelRow<T> row, Language? language = null)
        where T : struct, IExcelRow<T>
    {
        return (RowRef)row.AsTypedRowRef(language);
    }

    public static Dictionary<uint, uint> ToSingleLookup<TSourceSheet>(
        this ExcelSheet<TSourceSheet> sourceSheet,
        Func<TSourceSheet, uint> sourceSelector,
        Func<TSourceSheet, uint> lookupSelector,
        bool ignoreSourceZeroes = true,
        bool ignoreLookupZeroes = true)
        where TSourceSheet : struct, IExcelRow<TSourceSheet>
    {
        var dict = new Dictionary<uint, uint>();
        foreach (var item in sourceSheet)
        {
            var source = sourceSelector(item);
            if (source != 0 || !ignoreSourceZeroes)
            {
                var lookup = lookupSelector(item);
                if (lookup != 0 || !ignoreLookupZeroes)
                {
                    if (!dict.TryGetValue(source, out _))
                    {
                        dict.Add(source, lookup);
                    }
                }
            }
        }

        return dict;
    }

    public static Dictionary<uint, uint> ToSingleLookup<TSourceSheet>(
        this ExcelSheet<TSourceSheet> sourceSheet,
        Func<TSourceSheet, IEnumerable<uint>> sourceSelector,
        Func<TSourceSheet, uint> lookupSelector,
        bool ignoreSourceZeroes = true,
        bool ignoreLookupZeroes = true)
        where TSourceSheet : struct, IExcelRow<TSourceSheet>
    {
        var dict = new Dictionary<uint, uint>();
        foreach (var item in sourceSheet)
        {
            var sources = sourceSelector(item);
            foreach (var source in sources)
            {
                if (source != 0 || !ignoreSourceZeroes)
                {
                    var lookup = lookupSelector(item);
                    if (lookup != 0 || !ignoreLookupZeroes)
                    {
                        if (!dict.TryGetValue(source, out _))
                            dict.Add(source, lookup);
                    }
                }
            }
        }

        return dict;
    }
}