using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

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

}