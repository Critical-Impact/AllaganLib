using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemFateSource : ItemSource
{
    private FateRow fateRow;

    public ItemFateSource(ItemRow item, FateRow fate) : base(ItemInfoType.Fate)
    {
        this.Item = item;
        this.fateRow = fate;
    }

    public FateRow Fate => this.fateRow;

    public override uint Quantity => 1;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}