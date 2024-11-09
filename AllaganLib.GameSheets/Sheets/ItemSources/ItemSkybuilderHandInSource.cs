// <copyright file="ItemSkybuilderHandInSource.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemSkybuilderHandInSource : ItemSource
{
    private readonly int rowIndex;

    public HWDCrafterSupplyRow CrafterSupplyRow { get; }

    public ItemSkybuilderHandInSource(HWDCrafterSupplyRow crafterSupplyRow, int rowIndex, ItemRow item) : base(ItemInfoType.SkybuilderHandIn)
    {
        this.rowIndex = rowIndex;
        this.CrafterSupplyRow = crafterSupplyRow;
        this.Item = item;
    }

    public byte Level => this.CrafterSupplyRow.Base.HWDCrafterSupplyParams[this.rowIndex].Level;

    public byte LevelMax => this.CrafterSupplyRow.Base.HWDCrafterSupplyParams[this.rowIndex].LevelMax;

    public override uint Quantity => 1;

    public override string Name { get; }

    public override uint Icon { get; }
}