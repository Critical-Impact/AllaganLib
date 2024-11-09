using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public abstract class ItemDungeonSource : ItemSource
{
    public ContentFinderConditionRow ContentFinderCondition { get; }

    protected ItemDungeonSource(ContentFinderConditionRow contentFinderCondition, ItemInfoType infoType)
        : base(infoType)
    {
        this.ContentFinderCondition = contentFinderCondition;
    }
}