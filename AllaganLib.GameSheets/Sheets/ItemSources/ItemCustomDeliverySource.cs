using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemCustomDeliverySource : ItemSource
{
    public SatisfactionSupplyRow SupplyRow { get; }

    public ItemCustomDeliverySource(ItemRow item, SatisfactionSupplyRow supplyRow) : base(ItemInfoType.CustomDelivery)
    {
        this.Item = item;
        this.SupplyRow = supplyRow;
    }

    public override uint Quantity => 1;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}