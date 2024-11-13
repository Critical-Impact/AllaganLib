using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCustomDeliverySource : ItemSource
{
    public SatisfactionSupplyRow SupplyRow { get; }

    public ItemCustomDeliverySource(ItemRow item, SatisfactionSupplyRow supplyRow) : base(ItemInfoType.CustomDelivery)
    {
        this.Item = item;
        this.SupplyRow = supplyRow;
    }

    public override uint Quantity => 1;

}