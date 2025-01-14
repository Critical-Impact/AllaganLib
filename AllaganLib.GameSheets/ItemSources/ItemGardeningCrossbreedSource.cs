using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemGardeningCrossbreedSource : ItemSource
{
    public ItemRow SeedResult { get; }

    public ItemRow Seed1 { get; }

    public ItemRow Seed2 { get; }

    public ItemGardeningCrossbreedSource(ItemRow relatedItem, ItemRow seedResult, ItemRow seed1, ItemRow seed2) : base(ItemInfoType.GardeningCrossbreed)
    {
        this.Item = relatedItem;
        this.SeedResult = seedResult;
        this.Seed1 = seed1;
        this.Seed2 = seed2;
    }

    public override uint Quantity => 1;
}