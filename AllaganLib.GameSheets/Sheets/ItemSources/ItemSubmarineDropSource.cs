using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemSubmarineDropSource : ItemSource
{
    private readonly SubmarineExplorationRow submarineExploration;

    public ItemSubmarineDropSource(ItemRow item, SubmarineExplorationRow submarineExploration) : base(ItemInfoType.Submarine)
    {
        this.Item = item;
        this.submarineExploration = submarineExploration;
    }

    public SubmarineExplorationRow SubmarineExploration => this.submarineExploration;

    public override uint Quantity => 1;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}