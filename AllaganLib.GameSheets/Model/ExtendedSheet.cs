using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace AllaganLib.GameSheets.Model;

public abstract class ExtendedSheet<TBase, TExtendedRow, TExtendedSheet> : IReadOnlyList<TExtendedRow>
    where TBase : struct, IExcelRow<TBase>
    where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
    where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>,
    IExtendedSheet
{
    private Dictionary<uint, TExtendedRow> extendedRows;
    private Dictionary<(uint, ushort), TExtendedRow> extendedSubRows;
    private uint? startRow;

    public ExtendedSheet(
        GameData gameData,
        SheetManager sheetManager,
        SheetIndexer sheetIndexer,
        ItemInfoCache itemInfoCache)
    {
        this.GameData = gameData;
        this.BaseSheet = gameData.GetExcelSheet<TBase>(Language.English)!;
        this.SheetManager = sheetManager;
        this.SheetIndexer = sheetIndexer;
        this.ItemInfoCache = itemInfoCache;
        this.extendedRows = new Dictionary<uint, TExtendedRow>(this.BaseSheet.Count);
    }

    public GameData GameData { get; }

    public ExcelSheet<TBase> BaseSheet { get; }

    public SheetManager SheetManager { get; }

    public SheetIndexer SheetIndexer { get; }

    public ItemInfoCache ItemInfoCache { get; }

    public TBase this[int index] => this.BaseSheet.GetRow((uint)index);

    public uint StartRow
    {
        get
        {
            if (this.startRow == null)
            {
                var firstRow = this.BaseSheet.FirstOrDefault();
                this.startRow = firstRow.RowId;
            }

            return this.startRow.Value;
        }
    }

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

    public TExtendedRow? GetRowOrDefault(uint rowId)
    {
        if (!this.extendedRows.TryGetValue(rowId, out var result))
        {
            if (!this.BaseSheet.HasRow(rowId))
            {
                return null;
            }

            var newExtendedRow = new TExtendedRow();
            newExtendedRow.RowId = rowId;
            newExtendedRow.Sheet = (TExtendedSheet)this;
            this.extendedRows[rowId] = newExtendedRow;
            result = newExtendedRow;
        }

        return result;
    }



    public abstract void CalculateLookups();

    public IEnumerator<TExtendedRow> GetEnumerator()
    {
        foreach (var baseRow in this.BaseSheet)
        {
            yield return this.GetRow(baseRow.RowId);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int Count => this.BaseSheet.Count;

    TExtendedRow IReadOnlyList<TExtendedRow>.this[int index] => throw new NotImplementedException();
}
