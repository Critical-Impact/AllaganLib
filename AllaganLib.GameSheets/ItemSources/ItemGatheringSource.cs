using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemGatheringSource : ItemSource
{
    private readonly GatheringItemRow gatheringItem;

    public ItemGatheringSource(GatheringItemRow gatheringItem, ItemInfoType infoType)
        : base(infoType)
    {
        this.gatheringItem = gatheringItem;
        this.Item = gatheringItem.Item!;
    }

    public override uint Quantity => 1;


    public GatheringItemRow GatheringItem => this.gatheringItem;

    public override HashSet<uint>? MapIds => this.gatheringItem.MapIds;

    public override RelationshipType RelationshipType => RelationshipType.CollectedFrom;
}