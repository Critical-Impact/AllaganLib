using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class CompanyCraftSequenceSheet : ExtendedSheet<CompanyCraftSequence, CompanyCraftSequenceRow, CompanyCraftSequenceSheet>, IExtendedSheet
{
    private CompanyCraftPartSheet? companyCraftPartSheet;
    private CompanyCraftSupplyItemSheet? companyCraftSupplyItemSheet;
    private Dictionary<uint, CompanyCraftSequenceRow>? itemRows;

    public CompanyCraftSequenceSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }

    public CompanyCraftSequenceRow? GetByItemId(uint rowId)
    {
        if (this.itemRows == null)
        {
            this.itemRows = new Dictionary<uint, CompanyCraftSequenceRow>();
            foreach (var sequence in this)
            {
                this.itemRows[sequence.Base.ResultItem.RowId] = sequence;
            }
        }

        return this.itemRows.GetValueOrDefault(rowId);
    }

    public CompanyCraftPartSheet GetCompanyCraftPartSheet()
    {
        return this.companyCraftPartSheet ??=
            this.SheetManager.GetSheet<CompanyCraftPartSheet>();
    }

    public CompanyCraftSupplyItemSheet GetCompanyCraftSupplyItemSheet()
    {
        return this.companyCraftSupplyItemSheet ??= this.SheetManager
            .GetSheet<CompanyCraftSupplyItemSheet>();
    }
}