using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class SpearfishingItemRow : ExtendedRow<SpearfishingItem, SpearfishingItemRow, SpearfishingItemSheet>
{

    private ItemRow? itemRow;
    private SpearfishingNotebookRow? spearfishingNotebookRow;
    private List<GatheringPointBaseRow>? gatheringPoints;

    public string FishRecordType => this.Base.FishingRecordType.Value.Addon.Value.Text.ExtractText();

    public SpearfishingNotebookRow? SpearfishingNotebook
    {
        get
        {
            return this.spearfishingNotebookRow ??= this.GatheringPoints.FirstOrDefault()?.SpearfishingNotebook;
        }
    }

    public List<GatheringPointBaseRow> GatheringPoints => this.gatheringPoints ??= this.Sheet.GetGatheringPointBaseSheet()
        .GetGatheringPointBasesBySpearfishingItemId(this.RowId)?.ToList() ?? [];


    public ItemRow? ItemRow
    {
        get
        {
            return this.itemRow ??= this.Base.Item.IsValid && this.Base.Item.RowId != 0 ? this.Sheet.GetItemSheet().GetRow(this.Base.Item.RowId) : null;
        }
    }
}