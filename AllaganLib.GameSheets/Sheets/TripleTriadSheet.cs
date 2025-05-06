using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class TripleTriadSheet : ExtendedSheet<TripleTriad, TripleTriadRow, TripleTriadSheet>, IExtendedSheet
{
    private readonly NpcShopCache shopCache;
    private TripleTriadResidentSheet? tripleTriadResidentSheet;

    public TripleTriadSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache, NpcShopCache shopCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
        this.shopCache = shopCache;
    }

    public TripleTriadResidentSheet GetTripleTriadResidentSheet()
    {
        return this.tripleTriadResidentSheet ??= this.SheetManager.GetSheet<TripleTriadResidentSheet>();
    }

    public List<ENpcBaseRow> GetNpcsByTripleTriadId(uint tripleTriadId)
    {
        var npcs = this.shopCache.GetNpcsByTripleTriadId(tripleTriadId);
        if (npcs == null || npcs.Count == 0)
        {
            return [];
        }

        var eNpcBaseSheet = this.SheetManager.GetSheet<ENpcBaseSheet>();
        return npcs.Where(c => c != 0).Select(c => eNpcBaseSheet.GetRow(c)).ToList();
    }

    public override void CalculateLookups()
    {

    }
}

public class TripleTriadRow : ExtendedRow<TripleTriad, TripleTriadRow, TripleTriadSheet>
{
    private TripleTriadResidentRow? tripleTriadResidentRow;
    private List<ENpcBaseRow>? eNpcBaseRows;

    public TripleTriadResidentRow TripleTriadResidentRow
    {
        get
        {
            return this.tripleTriadResidentRow ??= this.Sheet.GetTripleTriadResidentSheet().GetRow(this.RowId);
        }
    }

    public List<ENpcBaseRow> ENpcBaseRows => this.eNpcBaseRows ??= this.Sheet.GetNpcsByTripleTriadId(this.RowId);
}