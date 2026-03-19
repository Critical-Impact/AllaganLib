using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class ContentFinderConditionSheet : ExtendedSheet<ContentFinderCondition, ContentFinderConditionRow,
    ContentFinderConditionSheet>, IExtendedSheet
{
    private ContentRouletteSheet? contentRouletteSheet; // Cache for ContentRouletteSheet
    private ExcelSheet<InstanceContent>? instanceContentSheet;
    private Dictionary<uint, uint>? instanceLookup;

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

    public ExcelSheet<InstanceContent> GetInstanceContentSheet()
    {
        return this.instanceContentSheet ??= this.GameData.GetExcelSheet<InstanceContent>()!;
    }

    public uint? GetRelatedInstanceContent(uint contentFinderConditionId)
    {
        this.instanceLookup ??= this.GetInstanceContentSheet().Where(c => c.RowId != 0).DistinctBy(c => c.ContentFinderCondition.RowId)
            .ToDictionary(c => c.ContentFinderCondition.RowId, c => c.RowId);
        return this.instanceLookup[contentFinderConditionId];
    }
}