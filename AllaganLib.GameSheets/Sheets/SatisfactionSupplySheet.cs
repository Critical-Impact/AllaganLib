using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SatisfactionSupplySheet : ExtendedSubrowSheet<SatisfactionSupply, SatisfactionSupplyRow, SatisfactionSupplySheet>, IExtendedSheet
{
    private SatisfactionNpcSheet? satisfactionNpcSheet;

    public SatisfactionSupplySheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer)
        : base(gameData, sheetManager, sheetIndexer)
    {
    }

    public SatisfactionNpcSheet GetSatisfactionNpcSheet()
    {
        return this.satisfactionNpcSheet ??= this.SheetManager.GetSheet<SatisfactionNpcSheet>();
    }

    public override void CalculateLookups()
    {
    }
}