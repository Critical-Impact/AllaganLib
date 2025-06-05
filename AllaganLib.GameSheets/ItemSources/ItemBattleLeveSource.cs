using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public sealed class ItemBattleLeveSource : ItemSource
{
    private readonly HashSet<uint>? mapIds;
    private readonly List<ItemRow> items;

    public ItemBattleLeveSource(RowRef<BattleLeve> battleLeve, RowRef<Leve> leve, RowRef<LeveRewardItem> leveRewardItem, int rewardItemIndex, RowRef<LeveRewardItemGroup> leveRewardItemGroup, int rewardItemGroupIndex, ItemRow item)
        : base(ItemInfoType.BattleLeve)
    {
        this.BattleLeve = battleLeve;
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

        this.items = new List<ItemRow>();
        foreach (var itemRowRef in this.LeveRewardItemGroup.Value.Item)
        {
            var itemId = itemRowRef.RowId;
            if (itemId == 0)
            {
                continue;
            }

            var itemRow = item.Sheet.GetRowOrDefault(itemId);
            if (itemRow != null)
            {
                this.items.Add(itemRow);
            }
        }
    }

    /// <inheritdoc/>
    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        List<ItemInfo> rewardItems = new List<ItemInfo>();
        for (var index = 0; index < this.LeveRewardItemGroup.Value.Item.Count; index++)
        {
            var itemRowRef = this.LeveRewardItemGroup.Value.Item[index];


            var itemId = itemRowRef.RowId;
            if (itemId == 0)
            {
                continue;
            }

            var count = this.LeveRewardItemGroup.Value.Count[index];
            var isHq = this.LeveRewardItemGroup.Value.IsHQ[index];

            var itemRow = this.Item.Sheet.GetRowOrDefault(itemId);
            if (itemRow != null)
            {
                rewardItems.Add(ItemInfo.Create(itemRow, count, isHq));
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
    /// Gets the related battle leve.
    /// </summary>
    public RowRef<BattleLeve> BattleLeve { get; }

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