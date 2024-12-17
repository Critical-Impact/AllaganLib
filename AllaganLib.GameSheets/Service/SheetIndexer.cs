using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using Lumina.Excel;

namespace AllaganLib.GameSheets.Service;

public class SheetIndexer
{
    public Dictionary<uint, TExtendedRow2> OneToOne<TBase, TExtendedRow, TExtendedSheet, TBase2, TExtendedRow2, TExtendedSheet2>(ExtendedSheet<TBase, TExtendedRow, TExtendedSheet> baseSheet, ExtendedSheet<TBase2, TExtendedRow2, TExtendedSheet2> relatedSheet, Func<TExtendedRow, (uint,TExtendedRow2?)> transformer)
        where TBase : struct, IExcelRow<TBase>
        where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
        where TBase2 : struct, IExcelRow<TBase2>
        where TExtendedRow2 : ExtendedRow<TBase2, TExtendedRow2, TExtendedSheet2>, new()
        where TExtendedSheet2 : ExtendedSheet<TBase2, TExtendedRow2, TExtendedSheet2>, IExtendedSheet
    {
        var index = new Dictionary<uint, TExtendedRow2>();
        foreach (var row in baseSheet)
        {
            var result = transformer.Invoke(row);
            if (result.Item1 == 0 || result.Item2 == null || result.Item2.RowId == 0)
            {
                continue;
            }

            if (!index.TryAdd(result.Item1, result.Item2))
            {
                //do something here
            }
        }

        return index;
    }

    public Dictionary<uint, TExtendedRow> OneToOne<TBase, TExtendedRow, TExtendedSheet, TBase2, TExtendedRow2, TExtendedSheet2>(ExtendedSheet<TBase, TExtendedRow, TExtendedSheet> baseSheet, ExtendedSheet<TBase2, TExtendedRow2, TExtendedSheet2> relatedSheet, Func<TExtendedRow, (uint,TExtendedRow?)> transformer)
        where TBase : struct, IExcelRow<TBase>
        where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
        where TBase2 : struct, IExcelRow<TBase2>
        where TExtendedRow2 : ExtendedRow<TBase2, TExtendedRow2, TExtendedSheet2>, new()
        where TExtendedSheet2 : ExtendedSheet<TBase2, TExtendedRow2, TExtendedSheet2>, IExtendedSheet
    {
        var index = new Dictionary<uint, TExtendedRow>();
        foreach (var row in baseSheet)
        {
            var result = transformer.Invoke(row);
            if (result.Item1 == 0 || result.Item2 == null || result.Item2.RowId == 0)
            {
                continue;
            }

            if (!index.TryAdd(result.Item1, result.Item2))
            {
                //do something here
            }
        }

        return index;
    }

    public Dictionary<uint, uint> OneToOneById<TBase, TExtendedRow, TExtendedSheet>(ExtendedSheet<TBase, TExtendedRow, TExtendedSheet> baseSheet, Func<TExtendedRow, (uint,uint)> transformer)
        where TBase : struct, IExcelRow<TBase>
        where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
    {
        var index = new Dictionary<uint, uint>();
        foreach (var row in baseSheet)
        {
            var result = transformer.Invoke(row);
            if (result.Item1 == 0 || result.Item2 == 0)
            {
                continue;
            }

            if (!index.TryAdd(result.Item1, result.Item2))
            {
                //do something here
            }
        }

        return index;
    }

    public Dictionary<uint, List<uint>> OneToManyById<TBase, TExtendedRow, TExtendedSheet>(ExtendedSheet<TBase, TExtendedRow, TExtendedSheet> baseSheet, Func<TExtendedRow, (uint,uint)> transformer)
        where TBase : struct, IExcelRow<TBase>
        where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
    {
        var index = new Dictionary<uint, List<uint>>();
        foreach (var row in baseSheet)
        {
            var result = transformer.Invoke(row);
            if (result.Item1 == 0 || result.Item2 == 0)
            {
                continue;
            }

            index.TryAdd(result.Item1, new List<uint>());
            index[result.Item1].Add(result.Item2);
        }

        return index;
    }

    public Dictionary<uint, List<uint>> OneToManyById<TBase, TExtendedRow, TExtendedSheet>(ExtendedSubrowSheet<TBase, TExtendedRow, TExtendedSheet> baseSheet, Func<TExtendedRow, (uint,uint)> transformer)
        where TBase : struct, IExcelSubrow<TBase>
        where TExtendedRow : ExtendedSubrow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSubrowSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
    {
        var index = new Dictionary<uint, List<uint>>();
        foreach (var row in baseSheet)
        {
            var result = transformer.Invoke(row);
            if (result.Item1 == 0 || result.Item2 == 0)
            {
                continue;
            }

            index.TryAdd(result.Item1, new List<uint>());
            index[result.Item1].Add(result.Item2);
        }

        return index;
    }

    public Dictionary<uint, List<TExtendedRow>> OneToMany<TBase, TExtendedRow, TExtendedSheet, TBase2, TExtendedRow2, TExtendedSheet2>(ExtendedSheet<TBase, TExtendedRow, TExtendedSheet> baseSheet, Func<TExtendedRow, TExtendedRow2?> transformer)
        where TBase : struct, IExcelRow<TBase>
        where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
        where TExtendedSheet2 : ExtendedSheet<TBase2, TExtendedRow2, TExtendedSheet2>, IExtendedSheet
        where TBase2 : struct, IExcelRow<TBase2>
        where TExtendedRow2 : ExtendedRow<TBase2, TExtendedRow2, TExtendedSheet2>, new()
    {
        var index = new Dictionary<uint, List<TExtendedRow>>();
        foreach (var row in baseSheet)
        {
            var result = transformer.Invoke(row);
            if (result == null || result.RowId == 0 || row.RowId == 0)
            {
                continue;
            }

            index.TryAdd(result.RowId, []);
            index[result.RowId].Add(row);
        }

        return index;
    }

    public Dictionary<uint, List<TExtendedRow>> OneToMany<TBase, TExtendedRow, TExtendedSheet, TBase2, TExtendedRow2, TExtendedSheet2>(ExtendedSheet<TBase, TExtendedRow, TExtendedSheet> baseSheet, Func<TExtendedRow, List<TExtendedRow2>> transformer)
        where TBase : struct, IExcelRow<TBase>
        where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
        where TExtendedSheet2 : ExtendedSheet<TBase2, TExtendedRow2, TExtendedSheet2>, IExtendedSheet
        where TBase2 : struct, IExcelRow<TBase2>
        where TExtendedRow2 : ExtendedRow<TBase2, TExtendedRow2, TExtendedSheet2>, new()
    {
        var index = new Dictionary<uint, List<TExtendedRow>>();
        foreach (var row in baseSheet)
        {
            var resultsList = transformer.Invoke(row);
            foreach (var result in resultsList)
            {
                if (result.RowId == 0 || row.RowId == 0)
                {
                    continue;
                }

                index.TryAdd(result.RowId, []);
                index[result.RowId].Add(row);
            }
        }

        return index;
    }

    public Dictionary<uint, List<TExtendedRow>> ManyToOneId<TBase, TExtendedRow, TExtendedSheet>(ExtendedSheet<TBase, TExtendedRow, TExtendedSheet> baseSheet, Func<TExtendedRow, uint> transformer)
        where TBase : struct, IExcelRow<TBase>
        where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
    {
        var index = new Dictionary<uint, List<TExtendedRow>>();
        foreach (var row in baseSheet)
        {
            var result = transformer.Invoke(row);
            if (result == 0 || row.RowId == 0)
            {
                continue;
            }

            index.TryAdd(result, []);
            index[result].Add(row);
        }

        return index;
    }

    public Dictionary<uint, List<TExtendedRow>> ManyToOneId<TBase, TExtendedRow, TExtendedSheet>(ExtendedSheet<TBase, TExtendedRow, TExtendedSheet> baseSheet, Func<TExtendedRow, IEnumerable<uint>> transformer)
        where TBase : struct, IExcelRow<TBase>
        where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
        where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
    {
        var index = new Dictionary<uint, List<TExtendedRow>>();
        foreach (var row in baseSheet)
        {
            var resultList = transformer.Invoke(row);
            foreach (var result in resultList)
            {
                if (result == 0 || row.RowId == 0)
                {
                    continue;
                }

                index.TryAdd(result, []);
                index[result].Add(row);
            }
        }

        return index;
    }
}