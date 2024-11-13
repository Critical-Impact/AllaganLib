using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SatisfactionSupplyRow : ExtendedSubrow<SatisfactionSupply, SatisfactionSupplyRow, SatisfactionSupplySheet>
{
    public SatisfactionNpcRow Npc => this.Sheet.GetSatisfactionNpcSheet().GetRow(this.RowId);
}