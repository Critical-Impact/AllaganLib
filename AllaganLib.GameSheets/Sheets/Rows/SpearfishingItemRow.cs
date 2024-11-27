using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class SpearfishingItemRow : ExtendedRow<SpearfishingItem, SpearfishingItemRow, SpearfishingItemSheet>
{

    private ItemRow? itemRow;
    private SpearfishingNotebookRow? spearfishingNotebookRow;

    public SpearfishingNotebookRow? SpearfishingNotebook
    {
        get
        {
            return this.spearfishingNotebookRow ??= this.Base.TerritoryType.RowId != 0
                ? this.Sheet.GetSpearfishingNoteBookSheet().GetRowOrDefault(this.Base.TerritoryType.RowId)
                : null;
        }
    }


    public ItemRow? ItemRow
    {
        get
        {
            return this.itemRow ??= this.Base.Item.IsValid && this.Base.Item.RowId != 0 ? this.Sheet.GetItemSheet().GetRow(this.Base.Item.RowId) : null;
        }
    }
}