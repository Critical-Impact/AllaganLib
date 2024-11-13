using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class NotoriousMonsterRow : ExtendedRow<NotoriousMonster, NotoriousMonsterRow, NotoriousMonsterSheet>
{
    public string RankFormatted()
    {
        return this.Base.Rank switch
        {
            1 => "B",
            2 => "A",
            3 => "S",
            _ => "N/A",
        };
    }
}