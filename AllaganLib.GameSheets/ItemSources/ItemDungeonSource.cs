using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemDungeonSource : ItemSource
{
    private HashSet<uint>? mapIds;

    public ContentFinderConditionRow ContentFinderCondition { get; }

    protected ItemDungeonSource(ContentFinderConditionRow contentFinderCondition, ItemInfoType infoType)
        : base(infoType)
    {
        this.ContentFinderCondition = contentFinderCondition;
        if (this.ContentFinderCondition.Base.TerritoryType.ValueNullable != null)
        {
            this.mapIds = [this.ContentFinderCondition.Base.TerritoryType.Value.Map.RowId];
        }
    }

    public override HashSet<uint>? MapIds => this.mapIds;

    public override RelationshipType RelationshipType => RelationshipType.DropsFrom;
}