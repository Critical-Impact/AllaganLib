using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SatisfactionNpcSheet : ExtendedSheet<SatisfactionNpc, SatisfactionNpcRow, SatisfactionNpcSheet>, IExtendedSheet
{
    public SatisfactionNpcSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }
    
    public override void CalculateLookups()
    {
    }
}