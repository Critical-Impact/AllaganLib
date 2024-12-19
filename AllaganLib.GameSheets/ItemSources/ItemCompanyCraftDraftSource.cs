using AllaganLib.GameSheets.Caches;
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

    public override uint Quantity { get; }
}