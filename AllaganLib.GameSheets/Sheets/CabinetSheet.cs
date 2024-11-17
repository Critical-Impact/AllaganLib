using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class CabinetSheet : ExtendedSheet<Cabinet, CabinetRow, CabinetSheet>, IExtendedSheet
{
    private int? cabinetSize;
    private CabinetCategorySheet? cabinetCategorySheet;

    public CabinetSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }

    public CabinetCategorySheet CabinetCategorySheet => this.cabinetCategorySheet ??= this.SheetManager.GetSheet<CabinetCategorySheet>();

    public int CabinetSize => this.cabinetSize ??= this.Count;
}