using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemAquariumSource : ItemSource
{
    public AquariumFishRow AquariumFish { get; }

    public ItemAquariumSource(ItemRow itemRow, AquariumFishRow aquariumFish)
        : base(ItemInfoType.Aquarium)
    {
        this.AquariumFish = aquariumFish;
        this.Item = itemRow;
    }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.UsedIn;
}