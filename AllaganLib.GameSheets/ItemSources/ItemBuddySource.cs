using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemBuddySource : ItemSource
{
    public RowRef<Buddy> Buddy { get; }

    public ItemBuddySource(ItemRow item, RowRef<Buddy> buddy)
        : base(ItemInfoType.BuddyItem)
    {
        this.Buddy = buddy;
        this.Item = item;
    }

    public override uint Quantity => 0;
}