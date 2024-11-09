using System;
using System.Collections;
using System.Collections.Generic;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel;

namespace AllaganLib.GameSheets.Model;

public abstract class ExtendedSubrowSheet<TBase, TExtendedRow, TExtendedSheet> : IReadOnlyList<TExtendedRow>
    where TBase : struct, IExcelSubrow<TBase>
    where TExtendedRow : ExtendedSubrow<TBase, TExtendedRow, TExtendedSheet>, new()
    where TExtendedSheet : ExtendedSubrowSheet<TBase, TExtendedRow, TExtendedSheet>,
    IExtendedSheet
{
    private Dictionary<uint, TExtendedRow> extendedRows;
    private Dictionary<(uint, ushort), TExtendedRow> extendedSubRows;

    public ExtendedSubrowSheet(
        GameData gameData,
        SheetManager sheetManager,
        SheetIndexer sheetIndexer)
    {
        this.GameData = gameData;
        this.BaseSheet = gameData.GetSubrowExcelSheet<TBase>()!;
        this.SheetManager = sheetManager;
        this.SheetIndexer = sheetIndexer;
        this.extendedRows = new Dictionary<uint, TExtendedRow>(this.BaseSheet.Count);
        this.extendedSubRows = new Dictionary<(uint, ushort), TExtendedRow>(this.BaseSheet.Count);
    }

    public GameData GameData { get; }

    public SubrowExcelSheet<TBase> BaseSheet { get; }

    public SheetManager SheetManager { get; }

    public SheetIndexer SheetIndexer { get; }

    public TBase this[uint index, ushort subrow] => this.BaseSheet.GetSubrow(index, subrow);

    public TExtendedRow GetRow(uint rowId)
    {
        if (!this.extendedRows.TryGetValue(rowId, out var result))
        {
            var newExtendedRow = new TExtendedRow();
            newExtendedRow.RowId = rowId;
            newExtendedRow.Sheet = (TExtendedSheet)this;
            this.extendedRows[rowId] = newExtendedRow;
            result = newExtendedRow;
        }

        return result;
    }

    public TExtendedRow GetRow(uint rowId, ushort subRowId)
    {
        var valueTuple = (rowId, subRowId);
        if (!this.extendedSubRows.TryGetValue(valueTuple, out var result))
        {
            var newExtendedRow = new TExtendedRow();
            newExtendedRow.RowId = rowId;
            newExtendedRow.SubRowId = subRowId;
            newExtendedRow.Sheet = (TExtendedSheet)this;
            this.extendedSubRows[valueTuple] = newExtendedRow;
            result = newExtendedRow;
        }

        return result;
    }

    public abstract void CalculateLookups();

    public IEnumerator<TExtendedRow> GetEnumerator()
    {
        foreach (var baseRow in this.BaseSheet)
        {
            foreach (var subRow in baseRow)
            {
                yield return this.GetRow(subRow.RowId, subRow.SubrowId);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int Count => this.BaseSheet.Count;

    TExtendedRow IReadOnlyList<TExtendedRow>.this[int index] => throw new NotImplementedException();
}

public interface IExtendedSheet
{
    public void CalculateLookups();
}