using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class ClassJobSheet : ExtendedSheet<ClassJob, ClassJobRow, ClassJobSheet>, IExtendedSheet
{
    private ClassJobCategorySheet? classJobCategorySheet;

    public ClassJobSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public ClassJobCategorySheet GetClassCategorySheet()
    {
        return classJobCategorySheet ??= this.SheetManager.GetSheet<ClassJobCategorySheet>();
    }

    public override void CalculateLookups()
    {
    }
}