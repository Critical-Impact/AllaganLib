using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class SubmarineExplorationRow : ExtendedRow<SubmarineExploration, SubmarineExplorationRow,
    SubmarineExplorationSheet>
{
    private List<ItemRow>? dropItems;
    private List<uint>? dropItemsIds;

    public uint? UnlockId
    {
        get
        {
            var unlock = this.Sheet.GetUnlockByPoint(this.RowId);
            return unlock;
        }
    }

    public SubmarineExplorationRow? Unlock
    {
        get
        {
            var unlock = this.Sheet.GetUnlockByPoint(this.RowId);
            if (unlock == null)
            {
                return null;
            }

            return this.Sheet.GetRow(unlock.Value);
        }
    }

    public List<uint> DropItemIds
    {
        get { return this.dropItemsIds ??= this.Sheet.GetItemsBySubmarineExploration(this.RowId); }
    }

    public List<ItemRow> DropItems
    {
        get
        {
            if (this.dropItems == null)
            {
                var itemSheet = this.Sheet.GetItemSheet();
                this.dropItems = this.Sheet.GetItemsBySubmarineExploration(this.RowId).Select(c => itemSheet.GetRow(c))
                    .ToList();
            }
            return this.dropItems;
        }
    }
}