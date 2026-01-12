using System.Collections.Generic;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public sealed class ItemCraftLeveUse : ItemSource
{
    private readonly HashSet<uint>? mapIds;
    private int rowIndex;

    public ItemCraftLeveUse(RowRef<CraftLeve> craftLeve, RowRef<Leve> leve, ItemRow item, int rowIndex)
        : base(ItemInfoType.CraftLeve)
    {
        this.CraftLeve = craftLeve;
        this.Leve = leve;
        this.Item = item;
        this.ParamGrow = new RowRef<ParamGrow>(item.Sheet.GameData.Excel, this.Leve.Value.ClassJobLevel);
        this.mapIds = new();
        this.rowIndex = rowIndex;
        var mapRowId = this.Leve.ValueNullable?.LevelStart.ValueNullable?.Map.RowId;
        if (mapRowId != 0 && mapRowId != null)
        {
            this.mapIds.Add(mapRowId.Value);
        }
    }

    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        var itemInfos = new List<ItemInfo>();

        for (var index = 0; index < this.Leve.Value.LeveRewardItem.Value.LeveRewardItemGroup.Count; index++)
        {
            var leveRewardItemGroup = this.Leve.Value.LeveRewardItem.Value.LeveRewardItemGroup[index];
            foreach (var itemRowRef in leveRewardItemGroup.Value.Item)
            {
                var itemId = itemRowRef.RowId;
                if (itemId == 0)
                {
                    continue;
                }

                var itemRow = this.Item.Sheet.GetRowOrDefault(itemId);
                if (itemRow != null)
                {
                    var count = leveRewardItemGroup.Value.Count[index];
                    var isHq = leveRewardItemGroup.Value.IsHQ[index];
                    itemInfos.Add(ItemInfo.Create(itemRow, count, isHq));
                }
            }
        }

        return itemInfos;
    }

    protected override IReadOnlyList<ItemInfo>? CreateCostItems()
    {
        var itemInfos = new List<ItemInfo>();

        for (var index = 0; index < this.CraftLeve.Value.Item.Count; index++)
        {
            var craftItem = this.CraftLeve.Value.Item[index];
            if (craftItem.RowId == 0)
            {
                continue;
            }

            var itemRow = this.Item.Sheet.GetRowOrDefault(craftItem.RowId);
            if (itemRow != null)
            {
                itemInfos.Add(ItemInfo.Create(itemRow, this.CraftLeve.Value.ItemCount[index]));
            }
        }
        return itemInfos;
    }

    public override HashSet<uint>? MapIds => this.mapIds;

    /// <summary>
    /// Gets the experience reward of the leve.
    /// </summary>
    public int ExpReward => (int)(this.ParamGrow.Value.ScaledQuestXP * (decimal)this.ParamGrow.Value.QuestExpModifier * (decimal)this.Leve.Value.ExpFactor) + 1;

    /// <summary>
    /// Gets the quantity of the item required.
    /// </summary>
    public override uint Quantity => this.CraftLeve.Value.ItemCount[this.rowIndex];

    /// <summary>
    /// Gets the related gathering leve.
    /// </summary>
    public RowRef<CraftLeve> CraftLeve { get; }

    /// <summary>
    /// Gets the related leve.
    /// </summary>
    public RowRef<Leve> Leve { get; }

    /// <summary>
    /// Gets the related param grow.
    /// </summary>
    public RowRef<ParamGrow> ParamGrow { get; }

    public override RelationshipType RelationshipType => RelationshipType.Required;

    public override RelationshipType? CostRelationshipType => RelationshipType.Rewards;
}