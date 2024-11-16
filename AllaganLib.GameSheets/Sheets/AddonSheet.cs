using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class AddonSheet : ExtendedSheet<Addon, AddonRow, AddonSheet>, IExtendedSheet
{
    public AddonSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache) : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }
}
public class AddonRow : ExtendedRow<Addon, AddonRow, AddonSheet>
{

}