using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public sealed class ItemBeastmasterFamiliarUse : ItemSource
{
    public ItemBeastmasterFamiliarUse(ItemRow item, RowRef<LuminaSheets.XBMPet> familiar, RowRef<Pet> pet)
        : base(ItemInfoType.BeastmasterFamiliar)
    {
        this.Item = item;
        this.Familiar = familiar;
        this.Pet = pet;
    }

    /// <summary>
    /// Gets the familiar unlocked by using this item.
    /// </summary>
    public RowRef<LuminaSheets.XBMPet> Familiar { get; }

    /// <summary>
    /// Gets the pet of the familiar, which holds its name and abilities.
    /// </summary>
    public RowRef<Pet> Pet { get; }

    public override uint Quantity => 1;

    public override RelationshipType RelationshipType => RelationshipType.UsedIn;
}
