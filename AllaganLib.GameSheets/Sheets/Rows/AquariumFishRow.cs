using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class AquariumFishRow : ExtendedRow<AquariumFish, AquariumFishRow, AquariumFishSheet>
{
    public AquariumSize Size => (AquariumSize)this.Base.Size;
}