// <copyright file="ItemGatheringSource.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

public class ItemGatheringSource : ItemSource
{
    private readonly GatheringItemRow gatheringItem;

    public ItemGatheringSource(GatheringItemRow gatheringItem, ItemInfoType infoType)
        : base(infoType)
    {
        this.gatheringItem = gatheringItem;
        this.Item = gatheringItem.Item!;
    }

    public override uint Quantity => 1;

    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => this.Item.Base.Icon;

    public GatheringItemRow GatheringItem => this.gatheringItem;

    public override HashSet<uint>? MapIds => this.gatheringItem.MapIds;
}