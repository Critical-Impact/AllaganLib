using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public sealed class ItemJobSoulCrystalUse : ItemSource
{
    public ItemJobSoulCrystalUse(ClassJobRow classJob, ClassJobRow? parentClassJob, ItemRow item)
        : base(ItemInfoType.JobSoulCrystal)
    {
        this.ClassJob = classJob;
        this.ParentClassJob = parentClassJob;
        this.Item = item;
    }

    /// <summary>
    /// Gets the class/job that equipping this soul crystal unlocks.
    /// </summary>
    public ClassJobRow ClassJob { get; }

    /// <summary>
    /// Gets the parent class/job this soul crystal upgrades from, or null for standalone jobs with no base class.
    /// </summary>
    public ClassJobRow? ParentClassJob { get; }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.UsedIn;
}