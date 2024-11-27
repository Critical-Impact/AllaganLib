using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemSkybuilderInspectionSource : ItemSource
{
    private readonly int itemIndex;

    public HWDGathererInspectionRow GathererInspectionRow { get; }

    public HWDGathererInspection.HWDGathererInspectionDataStruct InspectionData => this.GathererInspectionRow.Base.HWDGathererInspectionData[this.itemIndex];

    public ItemSkybuilderInspectionSource(HWDGathererInspectionRow gathererInspectionRow, int itemIndex, ItemRow item, ItemRow costItem)
        : base(ItemInfoType.SkybuilderInspection)
    {
        this.itemIndex = itemIndex;
        this.GathererInspectionRow = gathererInspectionRow;
        this.itemIndex = itemIndex;
        this.Item = item;
        this.CostItem = costItem;
    }

    public override uint Quantity => this.InspectionData.AmountRequired;

}