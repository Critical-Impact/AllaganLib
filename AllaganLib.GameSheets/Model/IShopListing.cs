using System.Collections.Generic;

namespace AllaganLib.GameSheets.Model
{
    public interface IShopListing
    {
        /// <summary>
        /// Gets the rewards of the current listing.
        /// </summary>
        /// <value>The rewards of the current listing.</value>
        IEnumerable<IShopListingItem> Rewards { get; }

        /// <summary>
        /// Gets the costs of the current listing.
        /// </summary>
        /// <value>The costs of the current listing.</value>
        IEnumerable<IShopListingItem> Costs { get; }
    }
}