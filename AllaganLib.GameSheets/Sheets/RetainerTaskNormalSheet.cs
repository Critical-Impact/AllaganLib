using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class
    RetainerTaskNormalSheet : ExtendedSheet<RetainerTaskNormal, RetainerTaskNormalRow, RetainerTaskNormalSheet>,
    IExtendedSheet
{
    private Dictionary<uint, uint> retainerTasksByRetainerTaskNormalId;
    private RetainerTaskSheet? retainerTaskSheet;

    public RetainerTaskNormalSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.retainerTasksByRetainerTaskNormalId = new Dictionary<uint, uint>();
    }

    public override void CalculateLookups()
    {
        this.retainerTasksByRetainerTaskNormalId = this.GameData.GetExcelSheet<RetainerTask>()!.ToSingleLookup(c => c.Task.RowId, c => c.RowId);
    }

    public RetainerTaskSheet GetRetainerTaskSheet()
    {
        return this.retainerTaskSheet ??= this.SheetManager.GetSheet<RetainerTaskSheet>();
    }

    public uint? GetRetainerTaskByRetainerTaskNormalId(uint retainerTaskNormalId)
    {
        return this.retainerTasksByRetainerTaskNormalId.GetValueOrDefault(retainerTaskNormalId);
    }
}