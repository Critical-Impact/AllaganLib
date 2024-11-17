using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class AquariumFishRow : ExtendedRow<AquariumFish, AquariumFishRow, AquariumFishSheet>
{
    public AquariumSize Size => (AquariumSize)this.Base.Size;
}