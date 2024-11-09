using System.Collections.Generic;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
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

public class RetainerTaskRandomRow : ExtendedRow<RetainerTaskRandom, RetainerTaskRandomRow, RetainerTaskRandomSheet>
{
    private RetainerTaskRow? retainerTaskRow;

    public RetainerTaskRow? RetainerTaskRow
    {
        get
        {
            var retainerTaskId = this.Sheet.GetRetainerTaskByRetainerTaskRandomId(this.RowId);
            if (retainerTaskId != null)
            {
                return this.retainerTaskRow ??= this.Sheet.GetRetainerTaskSheet().GetRow(retainerTaskId.Value);
            }

            return null;
        }
    }

    private string? formattedName;

    public string FormattedName
    {
        get
        {
            if (this.formattedName == null)
            {
                this.formattedName = this.Base.Name.ExtractText() + " - Lv " + (this.RetainerTaskRow?.Base.RetainerLevel.ToString() ?? "Unknown");
            }

            return this.formattedName;
        }
    }

    private string? nameString;

    public string NameString
    {
        get
        {
            if (this.nameString == null)
            {
                this.nameString = this.Base.Name.ExtractText();
            }

            return this.nameString;
        }
    }
}