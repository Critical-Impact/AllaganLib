using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemCompanyCraftResultSource : ItemSource
{
    public CompanyCraftSequenceRow CompanyCraftSequence { get; }

    public ItemCompanyCraftResultSource(ItemRow item, CompanyCraftSequenceRow companyCraftSequence)
        : base(ItemInfoType.FreeCompanyCraftRecipe)
    {
        this.Item = item;
        this.CompanyCraftSequence = companyCraftSequence;
    }

    public override uint Quantity => 1;

}