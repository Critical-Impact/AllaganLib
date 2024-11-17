using System.Text;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Model;

public class ShopListingItem : IShopListingItem
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ShopListingItem" /> class.
    /// </summary>
    /// <param name="itemSheet"></param>
    /// <param name="shopItem">The <see cref="IShopListing" /> the entry is for.</param>
    /// <param name="itemId">The item of the entry.</param>
    /// <param name="count">The count for the entry.</param>
    /// <param name="isHq">A value indicating whether the <c>item</c> is high-quality.</param>
    /// <param name="collectabilityRating">The collectability rating of the entry.</param>
    public ShopListingItem(
        ItemSheet itemSheet,
        IShopListing shopItem,
        uint itemId,
        uint count,
        bool? isHq = null,
        uint? collectabilityRating = null,
        uint? requiredFcRank = null)
    {
        this.Item = itemSheet.GetRow(itemId);
        this.Count = count;
        this.IsHq = isHq;
        this.CollectabilityRating = collectabilityRating;
        this.ShopItem = shopItem;
        this.FCRankRequired = requiredFcRank;
    }

    /// <summary>
    ///     Gets the <see cref="IShopListing" /> the current entry is for.
    /// </summary>
    /// <value>The <see cref="IShopListing" /> the current entry is for.</value>
    public IShopListing ShopItem { get; private set; }

    /// <summary>
    ///     Gets the item of the current listing entry.
    /// </summary>
    /// <value>The item of the current listing entry.</value>
    public ItemRow Item { get; }

    /// <summary>
    ///     Gets the count for the current listing entry.
    /// </summary>
    /// <value>The count for the current listing entry.</value>
    public uint Count { get; }

    /// <summary>
    ///     Gets a value indicating whether the item is high-quality.
    /// </summary>
    /// <value>A value indicating whether the item is high-quality.</value>
    public bool? IsHq { get; }

    /// <summary>
    ///     Gets the collectability rating for the item.
    /// </summary>
    /// <value>The collectability rating of the item.</value>
    public uint? CollectabilityRating { get; }

    public uint? FCRankRequired { get; }

    /// <summary>
    ///     Returns a string that represents the current <see cref="ShopListingItem" />.
    /// </summary>
    /// <returns>A string that represents the current <see cref="ShopListingItem" />.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        if (this.Count > 1)
        {
            sb.AppendFormat("{0} ", this.Count);
        }

        sb.Append(this.Item.Base.Singular.ExtractText());
        if (this.IsHq ?? false)
        {
            sb.Append(" (HQ)");
        }

        return sb.ToString();
    }
}