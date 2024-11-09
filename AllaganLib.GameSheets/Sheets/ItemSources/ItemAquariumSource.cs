using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

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

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}