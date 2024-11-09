using System.Collections.Generic;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemSpearfishingSource : ItemSource
{
    private readonly SpearfishingItemRow spearfishingItemRow;

    public ItemSpearfishingSource(SpearfishingItemRow spearfishingItemRow)
        : base(ItemInfoType.Spearfishing)
    {
        this.spearfishingItemRow = spearfishingItemRow;
        this.Item = spearfishingItemRow.ItemRow!;
    }

    public override uint Quantity => 1;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;

    public SpearfishingItemRow SpearfishingItemRow => this.spearfishingItemRow;

    public override HashSet<uint>? MapIds => [this.SpearfishingItemRow.Base.TerritoryType.ValueNullable?.Map.RowId ?? 0];
}