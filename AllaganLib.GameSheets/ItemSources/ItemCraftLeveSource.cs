using System.Collections.Generic;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public sealed class ItemCraftLeveSource : ItemSource
{
    private readonly HashSet<uint>? mapIds;

    public ItemCraftLeveSource(RowRef<CraftLeve> craftLeve, RowRef<Leve> leve, RowRef<LeveRewardItem> leveRewardItem, int rewardItemIndex, RowRef<LeveRewardItemGroup> leveRewardItemGroup, int rewardItemGroupIndex, ItemRow item)
        : base(ItemInfoType.CraftLeve)
    {
        this.CraftLeve = craftLeve;
        this.Leve = leve;
        this.LeveRewardItem = leveRewardItem;
        this.RewardItemIndex = rewardItemIndex;
        this.LeveRewardItemGroup = leveRewardItemGroup;
        this.RewardItemGroupIndex = rewardItemGroupIndex;
        this.Item = item;
        this.ParamGrow = new RowRef<ParamGrow>(item.Sheet.GameData.Excel, this.Leve.Value.ClassJobLevel);
        this.mapIds = new();
        var mapRowId = this.Leve.ValueNullable?.LevelStart.ValueNullable?.Map.RowId;
        if (mapRowId != 0 && mapRowId != null)
        {
            this.mapIds.Add(mapRowId.Value);
        }
    }

    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        var itemInfos = new List<ItemInfo>();

        var leveRewardItemGroup = this.LeveRewardItemGroup;
        for (var index = 0; index < leveRewardItemGroup.Value.Item.Count; index++)
        {
            var itemRowRef = leveRewardItemGroup.Value.Item[index];
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

        return itemInfos;
    }

    protected override IReadOnlyList<ItemInfo>? CreateCostItems()
    {
        List<ItemInfo> rewardItems = new List<ItemInfo>();
        for (var index = 0; index < this.CraftLeve.Value.Item.Count; index++)
        {
            var itemRowRef = this.CraftLeve.Value.Item[index];

            var itemId = itemRowRef.RowId;
            if (itemId == 0)
            {
                continue;
            }

            var count = this.CraftLeve.Value.ItemCount[index];

            var itemRow = this.Item.Sheet.GetRowOrDefault(itemId);
            if (itemRow != null)
            {
                rewardItems.Add(ItemInfo.Create(itemRow, count));
            }
        }
        return rewardItems.ToArray();
    }

    public override HashSet<uint>? MapIds => this.mapIds;

    /// <summary>
    /// Gets the probability of getting this particular loot group.
    /// </summary>
    public byte ProbabilityPercent => this.LeveRewardItem.Value.ProbabilityPercent[this.RewardItemIndex];

    /// <summary>
    /// Gets the experience reward of the leve.
    /// </summary>
    public int ExpReward => (int)(this.ParamGrow.Value.ScaledQuestXP * (decimal)this.ParamGrow.Value.QuestExpModifier * (decimal)this.Leve.Value.ExpFactor) + 1;

    /// <summary>
    /// Gets the quantity of the item rewarded.
    /// </summary>
    public override uint Quantity => this.LeveRewardItemGroup.Value.Count[this.RewardItemGroupIndex];

    /// <summary>
    /// Gets a value indicating whether the item is HQ or NQ.
    /// </summary>
    public bool IsHq => this.LeveRewardItemGroup.Value.IsHQ[this.RewardItemGroupIndex];

    /// <summary>
    /// Gets the related craft leve.
    /// </summary>
    public RowRef<CraftLeve> CraftLeve { get; }

    /// <summary>
    /// Gets the related leve.
    /// </summary>
    public RowRef<Leve> Leve { get; }

    /// <summary>
    /// Gets the related leve reward item.
    /// </summary>
    public RowRef<LeveRewardItem> LeveRewardItem { get; }

    /// <summary>
    /// Gets the related leve reward item group.
    /// </summary>
    public RowRef<LeveRewardItemGroup> LeveRewardItemGroup { get; }

    /// <summary>
    /// Gets the related param grow.
    /// </summary>
    public RowRef<ParamGrow> ParamGrow { get; }

    /// <summary>
    /// Gets the index of the item group/probability collection within the leve reward item.
    /// </summary>
    public int RewardItemIndex { get; }

    /// <summary>
    /// Gets the index of the item inside the leve reward item group.
    /// </summary>
    public int RewardItemGroupIndex { get; }
}