using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SpearfishingNotebookSheet : ExtendedSheet<SpearfishingNotebook, SpearfishingNotebookRow, SpearfishingNotebookSheet>, IExtendedSheet
{
    private TerritoryTypeSheet? territoryTypeSheet;

    public SpearfishingNotebookSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public TerritoryTypeSheet GetTerritoryTypeSheet()
    {
        return this.territoryTypeSheet ??= this.SheetManager.GetSheet<TerritoryTypeSheet>();
    }

    public override void CalculateLookups()
    {
    }
}