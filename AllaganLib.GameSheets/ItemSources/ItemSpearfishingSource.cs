using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

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


    public SpearfishingItemRow SpearfishingItemRow => this.spearfishingItemRow;

    public override HashSet<uint>? MapIds => this.SpearfishingItemRow.GatheringPoints.Select(c => c.SpearfishingNotebook?.TerritoryTypeRow?.Map?.RowId ?? 0).Where(c => c != 0).Distinct().ToHashSet();
}