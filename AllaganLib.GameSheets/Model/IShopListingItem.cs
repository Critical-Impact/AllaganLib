using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;


namespace AllaganLib.GameSheets.Model
{
    public interface IShopListingItem
    {
        /// <summary>
        /// Gets the item of the current listing entry.
        /// </summary>
        /// <value>The item of the current listing entry.</value>
        ItemRow Item { get; }

        /// <summary>
        ///     Gets the count for the current listing entry.
        /// </summary>
        /// <value>The count for the current listing entry.</value>
        uint Count { get; }

        /// <summary>
        ///     Gets a value indicating whether the item is high-quality.
        /// </summary>
        /// <value>A value indicating whether the item is high-quality.</value>
        bool? IsHq { get; }

        /// <summary>
        ///     Gets the collectability rating for the item.
        /// </summary>
        uint? CollectabilityRating { get; }

        /// <summary>
        ///     Gets the collectability rating for the item.
        /// </summary>
        uint? FCRankRequired { get; }
    }
}