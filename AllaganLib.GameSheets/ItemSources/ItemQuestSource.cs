using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemQuestSource : ItemSource
{
    private List<ItemRow> items;
    private List<ItemRow> costItems;
    public RowRef<Quest> Quest { get; }

    public SubrowRef<QuestClassJobReward>? QuestClassJobReward { get; }

    public int? QuestClassJobRewardSubRowId { get; }
    public int? QuestClassJobSupplySubRowId { get; set; }
    public SubrowRef<QuestClassJobSupply>? QuestClassJobSupplyRef { get; set; }

    public ItemQuestSource(
        ItemRow itemRow,
        RowRef<Quest> quest,
        SubrowRef<QuestClassJobReward>? questClassJobRewardRef = null,
        int? questClassJobRewardSubRowId = null)
        : this(itemRow, quest)
    {
        this.QuestClassJobReward = questClassJobRewardRef;
        this.QuestClassJobRewardSubRowId = questClassJobRewardSubRowId;
        if (questClassJobRewardRef.HasValue && questClassJobRewardSubRowId != null)
        {
            var questClassJobReward = questClassJobRewardRef.Value.Value[questClassJobRewardSubRowId.Value];
            foreach (var rewardItem in questClassJobReward.RewardItem)
            {
                if (rewardItem.RowId == 0)
                {
                    continue;
                }
                this.items.Add(this.Item.Sheet.GetRow(rewardItem.RowId));
            }


            var questClassJobRequirement = questClassJobRewardRef.Value.Value[questClassJobRewardSubRowId.Value];
            foreach (var requirementItem in questClassJobRequirement.RequiredItem)
            {
                if (requirementItem.RowId == 0)
                {
                    continue;
                }

                this.costItems.Add(this.Item.Sheet.GetRow(requirementItem.RowId));
            }
        }
    }

    public ItemQuestSource(
        ItemRow itemRow,
        RowRef<Quest> quest,
        SubrowRef<QuestClassJobSupply>? questClassJobSupplyRef = null,
        int? questClassJobSupplySubRowId = null)
        : this(itemRow, quest)
    {
        this.QuestClassJobSupplyRef = questClassJobSupplyRef;
        this.QuestClassJobSupplySubRowId = questClassJobSupplySubRowId;
        if (questClassJobSupplyRef.HasValue && questClassJobSupplySubRowId != null)
        {
            var questClassJobSupplyRequirement = questClassJobSupplyRef.Value.Value[questClassJobSupplySubRowId.Value];
            if (questClassJobSupplyRequirement.Item.RowId != 0)
            {
                this.costItems.Add(this.Item.Sheet.GetRow(questClassJobSupplyRequirement.Item.RowId));
            }
        }
    }

    public ItemQuestSource(ItemRow itemRow, RowRef<Quest> quest) : base(ItemInfoType.Quest)
    {
        this.Quest = quest;
        this.Item = itemRow;
        var rewardItems = new List<ItemRow>();
        var requirementItems = new List<ItemRow>();

        foreach (var reward in quest.Value.Reward)
        {
            if (reward.Is<Item>())
            {
                if (reward.RowId == 0)
                {
                    continue;
                }

                var item = itemRow.Sheet.GetRowOrDefault(reward.RowId);
                if (item is not null)
                {
                    rewardItems.Add(item);
                }
            }
        }

        foreach (var catalyst in this.Quest.Value.ItemCatalyst)
        {
            if (catalyst.RowId == 0)
            {
                continue;
            }
            rewardItems.Add(this.Item.Sheet.GetRow(catalyst.RowId));
        }

        foreach (var optionalReward in this.Quest.Value.OptionalItemReward)
        {
            if (optionalReward.RowId == 0)
            {
                continue;
            }
            rewardItems.Add(this.Item.Sheet.GetRow(optionalReward.RowId));
        }

        if (this.Quest.Value.QuestClassJobSupply.RowId != 0)
        {
            var a = "";
        }

        this.items = rewardItems;
        this.costItems = requirementItems;
    }

    public override List<ItemRow> Items => this.items;

    public override List<ItemRow> CostItems => this.costItems;

    public override uint Quantity => 1;

    public uint QuestIcon => this.Quest.Value.EventIconType.ValueNullable?.MapIconAvailable + 1 ?? 71021;
}