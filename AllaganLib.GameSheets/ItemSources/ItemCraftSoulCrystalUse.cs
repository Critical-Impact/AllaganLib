using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public sealed class ItemCraftSoulCrystalUse : ItemSource
{
    public ItemCraftSoulCrystalUse(ClassJobRow classJob, ItemRow item)
        : base(ItemInfoType.CraftSoulCrystal)
    {
        this.ClassJob = classJob;
        this.Item = item;
    }

    /// <summary>
    /// Gets the crafting class enabled by equipping this soul crystal.
    /// </summary>
    public ClassJobRow ClassJob { get; }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.UsedIn;
}
