using System.Collections.Generic;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public sealed class ItemCraftLeveUse : ItemSource
{
    private readonly HashSet<uint>? mapIds;
    private readonly List<ItemRow> items;
    private readonly List<ItemRow> costItems;
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

        this.items = new();
        foreach (var leveRewardItemGroup in this.Leve.Value.LeveRewardItem.Value.LeveRewardItemGroup)
        {
            foreach (var itemRowRef in leveRewardItemGroup.Value.Item)
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

        this.CostItem = item;
        this.costItems = [item];
        for (var index = 0; index < this.CraftLeve.Value.Item.Count; index++)
        {
            var craftItem = this.CraftLeve.Value.Item[index];
            if (craftItem.RowId == 0)
            {
                continue;
            }

            var itemRow = item.Sheet.GetRowOrDefault(craftItem.RowId);
            if (itemRow != null)
            {
                this.costItems.Add(itemRow);
            }
        }
    }

    public override List<ItemRow> Items => this.items;

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
}