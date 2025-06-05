using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Model;

public class ItemInfo
{
    public ItemInfo(ItemRow itemRow, uint? count = null, bool? isHighQuality = null, bool? isOptional = null, uint? min = null, uint? max = null, decimal? probability = null)
    {
        this.ItemRow = itemRow;
        this.Count = count;
        this.IsHighQuality = isHighQuality;
        if (isOptional.HasValue || min.HasValue || max.HasValue || probability.HasValue)
        {
            this.Extra = new ItemInfoExtra
            {
                IsOptional = isOptional,
                Min = min,
                Max = max,
                Probability = probability,
            };
        }
    }

    public uint ItemId => this.ItemRow.RowId;

    public static ItemInfo Create(ItemRow itemRow, uint? count = null, bool? isHighQuality = null, bool? isOptional = null, uint? min = null, uint? max = null, decimal? probability = null)
    {
        return new ItemInfo(itemRow, count, isHighQuality, isOptional, min, max, probability);
    }

    public static ItemInfo[] FromShopListing(IEnumerable<IShopListingItem> shopListingItems)
    {
        return shopListingItems.Select(FromShopListing).ToArray();
    }

    public static ItemInfo FromShopListing(IShopListingItem shopListingItem)
    {
        return new ItemInfo(shopListingItem.Item, shopListingItem.Count, shopListingItem.IsHq);
    }

    public ItemRow ItemRow { get; }

    public uint? Count { get; private init; }

    public bool? IsHighQuality { get; private init; }

    public bool? IsOptional => this.Extra?.IsOptional;

    public uint? Min => this.Extra?.Min;

    public uint? Max => this.Extra?.Max;

    public decimal? Probability => this.Extra?.Probability;

    private ItemInfoExtra? Extra { get; }

}

public class ItemInfoExtra
{
    public bool? IsOptional { get; init; }

    public uint? Min { get; init; }

    public uint? Max { get; init; }

    public decimal? Probability { get; init; }
}
