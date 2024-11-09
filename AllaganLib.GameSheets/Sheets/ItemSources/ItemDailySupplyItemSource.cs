// <copyright file="ItemDailySupplyItemSource.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemDailySupplyItemSource : ItemSource
{
    private readonly int supplyIndex;

    public DailySupplyItemRow DailySupplyItem { get; }

    public ItemDailySupplyItemSource(DailySupplyItemRow dailySupplyItem, int supplyIndex, ItemRow itemRow)
        : base(ItemInfoType.GCDailySupply)
    {
        this.supplyIndex = supplyIndex;
        this.DailySupplyItem = dailySupplyItem;
        this.Item = itemRow;
    }

    public override uint Quantity => this.DailySupplyItem.Base.Quantity[this.supplyIndex];

    public byte RecipeLevel => this.DailySupplyItem.Base.RecipeLevel[this.supplyIndex];

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}