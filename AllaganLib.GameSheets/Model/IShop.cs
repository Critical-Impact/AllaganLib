using System.Collections.Generic;

using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Model
{
    public interface IShop
    {
        /// <summary>
        ///     Gets the key of the current shop.
        /// </summary>
        /// <value>The key of the current shop.</value>
        uint RowId { get; }

        /// <summary>
        ///     Gets the name of the current shop.
        /// </summary>
        /// <value>The name of the current shop.</value>
        string Name { get; }

        IEnumerable<IShopListing> ShopListings { get; }

        IEnumerable<ENpcBaseRow> ENpcs { get; }

        IEnumerable<ItemRow> Items { get; }

        IEnumerable<ItemRow> CostItems { get; }
    }
}