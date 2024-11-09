using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class ContentFinderConditionSheet : ExtendedSheet<ContentFinderCondition, ContentFinderConditionRow,
    ContentFinderConditionSheet>, IExtendedSheet
{
    private ContentRouletteSheet? contentRouletteSheet; // Cache for ContentRouletteSheet

    public ContentFinderConditionSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }

    public ContentRouletteSheet GetContentRouletteSheet()
    {
        return this.contentRouletteSheet ??=
            this.SheetManager.GetSheet<ContentRouletteSheet>();
    }

    public ClassJobCategorySheet GetClassJobCategorySheet()
    {
        return this.SheetManager.GetSheet<ClassJobCategorySheet>();
    }
}