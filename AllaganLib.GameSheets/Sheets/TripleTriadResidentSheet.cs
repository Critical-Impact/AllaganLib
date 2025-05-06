using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class TripleTriadResidentSheet : ExtendedSheet<TripleTriadResident, TripleTriadResidentRow, TripleTriadResidentSheet>, IExtendedSheet
{
    public TripleTriadResidentSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {

    }

    public override void CalculateLookups()
    {

    }
}

public class TripleTriadResidentRow : ExtendedRow<TripleTriadResident, TripleTriadResidentRow, TripleTriadResidentSheet>
{

}