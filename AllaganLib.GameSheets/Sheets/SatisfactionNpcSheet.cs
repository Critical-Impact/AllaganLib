using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SatisfactionNpcSheet : ExtendedSheet<SatisfactionNpc, SatisfactionNpcRow, SatisfactionNpcSheet>, IExtendedSheet
{
    private Dictionary<uint, uint>? supplyIndexToNpcIdLookup;
    public SatisfactionNpcSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public SatisfactionNpcRow? GetRowBySupplyId(uint supplyId)
    {
        if (this.supplyIndexToNpcIdLookup == null)
        {
            this.supplyIndexToNpcIdLookup = new();
            foreach (var row in this)
            {
                foreach (var param in row.Base.SatisfactionNpcParams)
                {
                    this.supplyIndexToNpcIdLookup[(uint)param.SupplyIndex] = row.RowId;
                }
            }
        }

        if (this.supplyIndexToNpcIdLookup.TryGetValue(supplyId, out var rowId))
        {
            return this.GetRow(rowId);
        }

        return null;
    }

    public override void CalculateLookups()
    {
    }
}