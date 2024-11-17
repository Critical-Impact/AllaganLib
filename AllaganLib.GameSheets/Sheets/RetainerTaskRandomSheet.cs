using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class
    RetainerTaskRandomSheet : ExtendedSheet<RetainerTaskRandom, RetainerTaskRandomRow, RetainerTaskRandomSheet>,
    IExtendedSheet
{
    private Dictionary<uint, uint> retainerTasksByRetainerTaskRandomId;
    private RetainerTaskSheet? retainerTaskSheet;

    public RetainerTaskRandomSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.retainerTasksByRetainerTaskRandomId = new Dictionary<uint, uint>();
    }

    public override void CalculateLookups()
    {
        this.retainerTasksByRetainerTaskRandomId = this.GameData.GetExcelSheet<RetainerTask>()!.ToSingleLookup(c => c.Task.RowId, c => c.RowId);
    }

    public RetainerTaskSheet GetRetainerTaskSheet()
    {
        return this.retainerTaskSheet ??= this.SheetManager.GetSheet<RetainerTaskSheet>();
    }

    public uint? GetRetainerTaskByRetainerTaskRandomId(uint retainerTaskRandomId)
    {
        return this.retainerTasksByRetainerTaskRandomId.GetValueOrDefault(retainerTaskRandomId);
    }
}