using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

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

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}