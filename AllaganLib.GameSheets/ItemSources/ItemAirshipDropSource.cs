using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemAirshipDropSource : ItemSource
{
    private readonly AirshipExplorationPointRow airshipExplorationPoint;

    public ItemAirshipDropSource(ItemRow item, AirshipExplorationPointRow airshipExplorationPoint)
        : base(ItemInfoType.Airship)
    {
        this.Item = item;
        this.airshipExplorationPoint = airshipExplorationPoint;
    }

    public AirshipExplorationPointRow AirshipExplorationPoint => this.airshipExplorationPoint;

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.DropsFrom;
}