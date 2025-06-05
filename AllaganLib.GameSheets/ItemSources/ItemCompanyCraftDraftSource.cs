using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCompanyCraftDraftSource : ItemSource
{
    public RowRef<CompanyCraftDraft> CompanyCraftDraft { get; }

    public ItemCompanyCraftDraftSource(ItemRow itemRow, RowRef<CompanyCraftDraft> companyCraftDraft, uint quantityRequired)
        : base(ItemInfoType.CompanyCraftDraft)
    {
        this.CompanyCraftDraft = companyCraftDraft;
        this.Item = itemRow;
        this.Quantity = quantityRequired;
    }

    protected override IReadOnlyList<ItemInfo>? CreateCostItems()
    {
        var items = new List<ItemInfo>();
        for (var index = 0; index < this.CompanyCraftDraft.Value.RequiredItem.Count; index++)
        {
            var requiredItem = this.CompanyCraftDraft.Value.RequiredItem[index];
            if (requiredItem.RowId == 0)
            {
                continue;
            }

            var item = this.Item.Sheet.GetRowOrDefault(requiredItem.RowId);

            if (item != null)
            {
                var count = this.CompanyCraftDraft.Value.RequiredItemCount[index];
                items.Add(ItemInfo.Create(item, count));
            }
        }

        return items;
    }

    public override uint Quantity { get; }
}