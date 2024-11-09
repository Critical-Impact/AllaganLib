using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemAirshipDropSource : ItemSource
{
    private readonly AirshipExplorationPointRow airshipExplorationPoint;

    public ItemAirshipDropSource(ItemRow item, AirshipExplorationPointRow airshipExplorationPoint) : base(ItemInfoType.Airship)
    {
        this.Item = item;
        this.airshipExplorationPoint = airshipExplorationPoint;
    }

    public AirshipExplorationPointRow AirshipExplorationPoint => this.airshipExplorationPoint;

    public override uint Quantity => 1;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}