using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class GrandCompanyRow : ExtendedRow<GrandCompany, GrandCompanyRow, GrandCompanySheet>
{
    public uint ItemId
    {
        get
        {
            return this.Base.RowId switch
            {
                1 => HardcodedItems.StormSealId,
                2 => HardcodedItems.SerpentSealId,
                3 => HardcodedItems.FlameSealId,
                _ => 1,
            };
        }
    }
}