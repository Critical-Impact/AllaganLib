using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

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