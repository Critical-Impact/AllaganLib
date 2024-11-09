using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
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

public class RetainerTaskNormalRow : ExtendedRow<RetainerTaskNormal, RetainerTaskNormalRow, RetainerTaskNormalSheet>
{
    private RetainerTaskRow? retainerTaskRow;

    public RetainerTaskRow? RetainerTaskRow
    {
        get
        {
            var retainerTaskId = this.Sheet.GetRetainerTaskByRetainerTaskNormalId(this.RowId);
            if (retainerTaskId != null)
            {
                return this.retainerTaskRow ??= this.Sheet.GetRetainerTaskSheet().GetRow(retainerTaskId.Value);
            }

            return null;
        }
    }

    public string TaskName
    {
        get
        {
            if (this.RetainerTaskRow != null)
            {
                var classJobName = this.RetainerTaskRow.ClassJobCategoryRow?.Base.Name.ToString();
                var level = this.RetainerTaskRow.Base.RetainerLevel;
                return classJobName + " - Lv " + level;
            }

            return "Unknown";
        }
    }

    public ushort TaskTime
    {
        get
        {
            if (this.RetainerTaskRow != null)
            {
                return this.RetainerTaskRow.Base.MaxTimemin;
            }

            return 0;
        }
    }

    public string Quantities
    {
        get
        {
            return string.Join(", ", this.Base.Quantity.Where(c => c != 0).Select(c => c.ToString()));
        }
    }
}