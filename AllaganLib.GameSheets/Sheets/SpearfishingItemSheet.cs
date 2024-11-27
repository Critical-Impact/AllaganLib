using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SpearfishingItemSheet : ExtendedSheet<SpearfishingItem, SpearfishingItemRow, SpearfishingItemSheet>,
    IExtendedSheet
{
    private ItemSheet? itemSheet;
    private SpearfishingNotebookSheet? spearFishingNotebookSheet;

    public SpearfishingItemSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(
        gameData,
        sheetManager,
        sheetIndexer,
        itemInfoCache)
    {
    }

    public ItemSheet GetItemSheet()
    {
        return this.itemSheet ??= this.SheetManager.GetSheet<ItemSheet>();
    }

    public SpearfishingNotebookSheet GetSpearfishingNoteBookSheet()
    {
        return this.spearFishingNotebookSheet ??= this.SheetManager.GetSheet<SpearfishingNotebookSheet>();
    }

    public override void CalculateLookups()
    {
    }
}