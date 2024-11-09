using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class EquipSlotCategorySheet : ExtendedSheet<EquipSlotCategory, EquipSlotCategoryRow, EquipSlotCategorySheet>,
    IExtendedSheet
{
    public EquipSlotCategorySheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(
            gameData,
            sheetManager,
            sheetIndexer,
            itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }
}