// <copyright file="ItemSkybuilderInspectionSource.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemSkybuilderInspectionSource : ItemSource
{
    private readonly int itemIndex;

    public HWDGathererInspectionRow GathererInspectionRow { get; }

    public HWDGathererInspection.HWDGathererInspectionDataStruct InspectionData => this.GathererInspectionRow.Base.HWDGathererInspectionData[this.itemIndex];

    public ItemSkybuilderInspectionSource(HWDGathererInspectionRow gathererInspectionRow, int itemIndex, ItemRow item, ItemRow costItem) : base(ItemInfoType.SkybuilderInspection)
    {
        this.itemIndex = itemIndex;
        this.GathererInspectionRow = gathererInspectionRow;
        this.itemIndex = itemIndex;
        this.Item = item;
        this.CostItem = costItem;
    }

    public override uint Quantity => this.InspectionData.AmountRequired;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;
}