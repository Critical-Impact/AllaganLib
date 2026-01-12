#pragma warning disable PendingExcelSchema
using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;
using Quest = Lumina.Excel.Sheets.Experimental.Quest;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemQuestUse : ItemQuestSource
{
    public ItemQuestUse(ItemRow itemRow, List<QuestRequiredItem> requiredItems, RowRef<Quest> quest, SubrowRef<QuestClassJobReward>? questClassJobRewardRef = null, int? questClassJobRewardSubRowId = null) : base(itemRow, requiredItems, quest, questClassJobRewardRef, questClassJobRewardSubRowId)
    {
    }

    public ItemQuestUse(ItemRow itemRow, List<QuestRequiredItem> requiredItems, RowRef<Quest> quest, SubrowRef<QuestClassJobSupply>? questClassJobSupplyRef = null, int? questClassJobSupplySubRowId = null) : base(itemRow, requiredItems, quest, questClassJobSupplyRef, questClassJobSupplySubRowId)
    {
    }

    public ItemQuestUse(ItemRow itemRow, List<QuestRequiredItem> requiredItems, RowRef<Quest> quest) : base(itemRow, requiredItems, quest)
    {
    }

    public override RelationshipType RelationshipType => RelationshipType.Required;
    public override RelationshipType? CostRelationshipType => RelationshipType.Rewards;
}

public class ItemQuestSource : ItemSource
{
    public RowRef<Quest> Quest { get; }

    public SubrowRef<QuestClassJobReward>? QuestClassJobReward { get; }

    public int? QuestClassJobRewardSubRowId { get; }
    public int? QuestClassJobSupplySubRowId { get; set; }

    public List<QuestRequiredItem> RequiredItems { get; }

    public SubrowRef<QuestClassJobSupply>? QuestClassJobSupplyRef { get; set; }

    public ItemQuestSource(
        ItemRow itemRow,
        List<QuestRequiredItem> requiredItems,
        RowRef<Quest> quest,
        SubrowRef<QuestClassJobReward>? questClassJobRewardRef = null,
        int? questClassJobRewardSubRowId = null)
        : this(itemRow, requiredItems, quest)
    {
        this.RequiredItems = requiredItems;
        this.QuestClassJobReward = questClassJobRewardRef;
        this.QuestClassJobRewardSubRowId = questClassJobRewardSubRowId;
    }

    public ItemQuestSource(
        ItemRow itemRow,
        List<QuestRequiredItem> requiredItems,
        RowRef<Quest> quest,
        SubrowRef<QuestClassJobSupply>? questClassJobSupplyRef = null,
        int? questClassJobSupplySubRowId = null)
        : this(itemRow, requiredItems, quest)
    {
        this.RequiredItems = requiredItems;
        this.QuestClassJobSupplyRef = questClassJobSupplyRef;
        this.QuestClassJobSupplySubRowId = questClassJobSupplySubRowId;
    }

    public ItemQuestSource(ItemRow itemRow, List<QuestRequiredItem> requiredItems, RowRef<Quest> quest) : base(ItemInfoType.Quest)
    {
        this.RequiredItems = requiredItems;
        this.Quest = quest;
        this.Item = itemRow;
    }

    public override uint Quantity => 1;

    protected override IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        var itemInfos = new List<ItemInfo>();
        var itemSheet = this.Item.Sheet;

        for (var index = 0; index < this.Quest.Value.Reward.Count; index++)
        {
            var reward = this.Quest.Value.Reward[index];
            if (reward.Is<Item>())
            {
                if (reward.RowId == 0)
                {
                    continue;
                }

                var item = itemSheet.GetRowOrDefault(reward.RowId);
                if (item is not null)
                {
                    var count = this.Quest.Value.ItemCountReward[index];
                    itemInfos.Add(ItemInfo.Create(item, count));
                }
            }
        }

        if (this.QuestClassJobReward != null && this.QuestClassJobRewardSubRowId != null)
        {
            var jobReward =  this.QuestClassJobReward.Value.Value[this.QuestClassJobRewardSubRowId.Value];
            for (var index = 0; index < jobReward.RewardItem.Count; index++)
            {
                var rewardItem = jobReward.RewardItem[index];
                if (rewardItem.RowId == 0)
                {
                    continue;
                }

                var item = itemSheet.GetRowOrDefault(rewardItem.RowId);
                if (item is not null)
                {
                    var count = jobReward.RewardAmount[index];
                    itemInfos.Add(ItemInfo.Create(item, count));
                }
            }
        }

        for (var index = 0; index < this.Quest.Value.ItemCatalyst.Count; index++)
        {
            var catalyst = this.Quest.Value.ItemCatalyst[index];
            var catalystCount = this.Quest.Value.ItemCountCatalyst[index];
            if (catalyst.RowId == 0)
            {
                continue;
            }
            var item = itemSheet.GetRowOrDefault(catalyst.RowId);
            if (item is not null)
            {
                var count = catalystCount;
                itemInfos.Add(ItemInfo.Create(item, count));
            }
        }

        for (var index = 0; index < this.Quest.Value.OptionalItemReward.Count; index++)
        {
            var optionalReward = this.Quest.Value.OptionalItemReward[index];
            var optionalRewardCount = this.Quest.Value.OptionalItemCountReward[index];
            if (optionalReward.RowId == 0)
            {
                continue;
            }
            var item = itemSheet.GetRowOrDefault(optionalReward.RowId);
            if (item is not null)
            {
                var count = optionalRewardCount;
                itemInfos.Add(ItemInfo.Create(item, count, null, true));
            }
        }
        return itemInfos;
    }

    protected override IReadOnlyList<ItemInfo>? CreateCostItems()
    {
        var itemInfos = new List<ItemInfo>();
        foreach (var requiredItem in this.RequiredItems)
        {
            var item = this.Item.Sheet.GetRowOrDefault(requiredItem.ItemId);
            if (item != null)
            {
                itemInfos.Add(ItemInfo.Create(item, requiredItem.Quantity, requiredItem.IsHq));
            }
        }

        if (this.QuestClassJobSupplyRef != null && this.QuestClassJobSupplySubRowId != null)
        {
            var jobSupply = this.QuestClassJobSupplyRef.Value.Value[this.QuestClassJobSupplySubRowId.Value];
            var requiredItem = jobSupply.Item;
            if (requiredItem.RowId != 0)
            {
                var item = this.Item.Sheet.GetRowOrDefault(requiredItem.RowId);
                if (item is not null)
                {
                    itemInfos.Add(ItemInfo.Create(item, jobSupply.AmountRequired, jobSupply.ItemHQ));
                }
            }
        }
        return itemInfos;
    }

    public uint QuestIcon => this.Quest.Value.EventIconType.ValueNullable?.MapIconAvailable + 1 ?? 71021;

    public override RelationshipType RelationshipType => RelationshipType.Rewards;
    public override RelationshipType? CostRelationshipType => RelationshipType.Required;
}