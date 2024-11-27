using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemBuddySource : ItemSource
{
    public RowRef<BuddyItem> BuddyItem { get; }

    public ItemBuddySource(ItemRow item, RowRef<BuddyItem> buddy)
        : base(ItemInfoType.BuddyItem)
    {
        this.BuddyItem = buddy;
        this.Item = item;
    }

    public override uint Quantity => 0;
}