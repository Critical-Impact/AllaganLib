using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class ENpcResidentSheet : ExtendedSheet<ENpcResident, ENpcResidentRow, ENpcResidentSheet>, IExtendedSheet
{
    private ENpcBaseSheet? eNpcBaseSheet;

    public ENpcResidentSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public ENpcBaseSheet GetENpcBaseSheet()
    {
        return this.eNpcBaseSheet ??= this.SheetManager.GetSheet<ENpcBaseSheet>();
    }

    public override void CalculateLookups()
    {
    }
}