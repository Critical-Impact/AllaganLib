using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemAchievementSource : ItemSource
{
    public RowRef<Achievement> Achievement { get; }

    public ItemAchievementSource(ItemRow item, RowRef<Achievement> achievement)
        : base(ItemInfoType.Achievement)
    {
        this.Achievement = achievement;
        this.Item = item;
    }

    public override uint Quantity => 0;

    public override RelationshipType RelationshipType => RelationshipType.Rewards;
}