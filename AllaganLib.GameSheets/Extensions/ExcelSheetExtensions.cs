using System;
using System.Collections.Generic;
using Lumina.Excel;

namespace AllaganLib.GameSheets.Extensions;

public static class ExcelSheetExtensions
{
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