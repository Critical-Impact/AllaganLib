using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class SpecialShopListing : IShopListing
{
    private static Dictionary<uint, uint> currencies = new Dictionary<uint, uint>()
    {
        { 1, 10309 },
        { 2, 33913 }, // Unlimited Crafters{  scrip
        { 3, 10311 },
        { 4, 33914 }, // Unlimited Gatherers{  scrip
        { 5, 10307 },
        { 6, 41784 }, // Limited Crafters{  scrip
        { 7, 41785 }, // Limited Gatherers{  scrip
        { 8, 21072 },
        { 9, 21073 },
        { 10, 21074 },
        { 11, 21075 },
        { 12, 21076 },
        { 13, 21077 },
        { 14, 21078 },
        { 15, 21079 },
        { 16, 21080 },
        { 17, 21081 },
        { 18, 21172 },
        { 19, 21173 },
        { 20, 21935 },
        { 21, 22525 },
        { 22, 26533 },
        { 23, 26807 },
        { 24, 28063 },
        { 25, 28186 },
        { 26, 28187 },
        { 27, 28188 },
        { 28, 30341 }
    };

    private static Dictionary<uint, uint> tomeStones = new Dictionary<uint, uint>() {
        { 1, 28 },
        { 2, 46 },
        { 3, 47 },
    };

    public SpecialShopListing(
        SpecialShopRow specialShopRow,
        ItemSheet itemSheet,
        Dictionary<uint, uint> tomeStoneLookup,
        SpecialShop.ItemStruct itemDataStruct)
    {
        var costListings = new List<ShopListingItem>();
        foreach (var costItem in itemDataStruct.ItemCosts)
        {
            if (costItem.ItemCost.RowId != 0)
            {
                var costItemId = costItem.ItemCost.RowId;
                costItemId = this.ConvertCurrencyId(specialShopRow.RowId, costItemId, specialShopRow.Base.UseCurrencyType);
                costListings.Add(
                    new ShopListingItem(
                        itemSheet,
                        this,
                        costItemId,
                        costItem.CurrencyCost,
                        costItem.HqCost == 1,
                        costItem.CollectabilityCost));
            }
        }

        var rewardListings = new List<ShopListingItem>();
        foreach (var receiveItem in itemDataStruct.ReceiveItems)
        {
            if (receiveItem.Item.RowId != 0)
            {
                var rewardItemId = receiveItem.Item.RowId;
                rewardListings.Add(
                    new ShopListingItem(
                        itemSheet,
                        this,
                        rewardItemId,
                        receiveItem.ReceiveCount,
                        receiveItem.ReceiveHq));
            }
        }

        this.Rewards = rewardListings;
        this.Costs = costListings;
    }

    private uint ConvertCurrencyId(uint specialShopId, uint itemId, ushort useCurrencyType)
    {
        if (specialShopId == 1770637)
        {
            if (currencies.TryGetValue(itemId, out var currencyValue))
            {
                return currencyValue;
            }
        }

        if (specialShopId == 1770446 || (specialShopId == 1770699 && itemId < 10) || (specialShopId == 1770803 && itemId < 10))
        {
            if (tomeStones.TryGetValue(itemId, out var currencyValue) ||
                currencies.TryGetValue(itemId, out currencyValue))
            {
                return currencyValue;
            }
        }

        if (useCurrencyType == 16 && itemId != 25)
        {
            if (currencies.TryGetValue(itemId, out var currencyValue))
            {
                itemId = currencyValue;
            }
        }

        if (useCurrencyType == 2 && itemId < 10)
        {
            if (tomeStones.TryGetValue(itemId, out var tomestoneValue))
            {
                itemId = tomestoneValue;
            }
        }

        if (specialShopId == 1770637 && itemId < 10)
        {
            if (currencies.TryGetValue(itemId, out var currencyValue))
            {
                itemId = currencyValue;
            }
        }

        if ((useCurrencyType == 16 || useCurrencyType == 4) && itemId < 10 && specialShopId != 1770637)
        {
            if (tomeStones.TryGetValue(itemId, out var currencyValue) ||
                currencies.TryGetValue(itemId, out currencyValue))
            {
                itemId = currencyValue;
            }
        }

        return itemId;
    }

    public IEnumerable<IShopListingItem> Rewards { get; set; }

    public IEnumerable<IShopListingItem> Costs { get; set; }
}