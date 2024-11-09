// <copyright file="ItemCraftLeveSource.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemCraftLeveSource : ItemSource
{
    private readonly int rowIndex;

    public CraftLeveRow CraftLeveRow { get; }

    public ItemCraftLeveSource(CraftLeveRow craftLeveRow, int rowIndex, ItemRow item) : base(ItemInfoType.CraftLeve)
    {
        this.rowIndex = rowIndex;
        this.CraftLeveRow = craftLeveRow;
        this.Item = item;
    }

    public override uint Quantity => this.CraftLeveRow.Base.ItemCount[this.rowIndex];

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}