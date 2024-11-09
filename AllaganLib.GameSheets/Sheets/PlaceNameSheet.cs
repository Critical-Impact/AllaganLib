using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Caches;
using Lumina;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class PlaceNameSheet : ExtendedSheet<PlaceName, PlaceNameRow, PlaceNameSheet>, IExtendedSheet
{
    public PlaceNameSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }
}

public class PlaceNameRow : ExtendedRow<PlaceName, PlaceNameRow, PlaceNameSheet>
{
    private string? formattedName;

    public string FormattedName
    {
        get
        {
            if (this.formattedName == null)
            {
                this.formattedName = this.Base.Name.ExtractText();
            }

            return this.formattedName;
        }
    }
}