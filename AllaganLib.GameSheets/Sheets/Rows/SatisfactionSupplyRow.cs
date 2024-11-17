using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class SatisfactionSupplyRow : ExtendedSubrow<SatisfactionSupply, SatisfactionSupplyRow, SatisfactionSupplySheet>
{
    public SatisfactionNpcRow? Npc => this.Sheet.GetSatisfactionNpcSheet().GetRowBySupplyId(this.Base.RowId);
}