using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGCExpertDeliverySource : ItemSource
{
    public GCSupplyDutyRewardRow RewardRow { get; }

    public ItemGCExpertDeliverySource(ItemRow row, GCSupplyDutyRewardRow rewardRow)
        : base(ItemInfoType.GCExpertDelivery)
    {
        this.RewardRow = rewardRow;
        this.Item = row;
    }

    public uint SealsRewarded => this.RewardRow.Base.SealsExpertDelivery;

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.Rewards;
}