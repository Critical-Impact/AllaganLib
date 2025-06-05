using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemSource : GenericItemSource
{
    private IReadOnlyList<ItemInfo>? rewardItems;
    private IReadOnlyList<ItemInfo>? costItems;
    private bool? hasRewardItems;
    private bool? hasCostItems;

    public ItemRow Item { get; protected init; }

    public ItemRow? CostItem { get; protected init; }

    /// <summary>
    /// Gets a list of reward items for the source.
    /// </summary>
    public IReadOnlyList<ItemInfo> RewardItems
    {
        get
        {
            if (this.hasRewardItems == false)
            {
                return [ItemInfo.Create(this.Item)];
            }

            if (this.rewardItems != null)
            {
                return this.rewardItems;
            }

            var calculatedItems = this.CreateRewardItems();
            if (calculatedItems == null)
            {
                this.hasRewardItems = false;
                return [];
            }

            this.rewardItems = calculatedItems;
            this.hasRewardItems = true;

            return this.rewardItems;
        }
    }

    /// <summary>
    /// Gets a list of cost items for the source.
    /// </summary>
    public IReadOnlyList<ItemInfo> CostItems
    {
        get
        {
            if (this.hasCostItems == false)
            {
                return this.CostItem != null ? [ItemInfo.Create(this.CostItem)] : [];
            }

            if (this.costItems != null)
            {
                return this.costItems;
            }

            var calculatedItems = this.CreateCostItems();
            if (calculatedItems == null)
            {
                this.hasCostItems = false;
                return [];
            }

            this.costItems = calculatedItems;
            this.hasCostItems = true;

            return this.costItems;
        }
    }

    /// <summary>
    /// Creates the reward items for the source.
    /// </summary>
    /// <returns>A list of rewards.</returns>
    protected virtual IReadOnlyList<ItemInfo>? CreateRewardItems()
    {
        return null;
    }

    /// <summary>
    /// Creates the cost items for the source.
    /// </summary>
    /// <returns>A list of costs.</returns>
    protected virtual IReadOnlyList<ItemInfo>? CreateCostItems()
    {
        return null;
    }

    public override HashSet<uint>? MapIds => null;

    public ItemSource(ItemInfoType infoType)
    {
        this.Type = infoType;
    }
}