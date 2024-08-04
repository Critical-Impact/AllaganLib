using System;
using System.Collections.Generic;
using System.Globalization;

namespace AllaganLib.Universalis.Models;

public class UniversalisPricing
{
    public uint ItemId { get; set; }

    public uint WorldId { get; set; }

    public float AveragePriceNq { get; set; }

    public float AveragePriceHq { get; set; }

    public float MinPriceNq { get; set; }

    public float MinPriceHq { get; set; }

    public int SevenDaySellCount { get; set; }

    public int Available { get; set; }

    public DateTime? LastSellDate { get; set; }

    public DateTime LastUpdate { get; set; } = DateTime.Now;

    public UniversalisHistory[]? recentHistory;
    public UniversalisListing[]? listings;

    public void FromCsv(string[] lineData)
    {
        this.ItemId = uint.Parse(lineData[0], CultureInfo.InvariantCulture);
        this.WorldId = uint.Parse(lineData[1], CultureInfo.InvariantCulture);
        this.AveragePriceNq = float.Parse(lineData[2], CultureInfo.InvariantCulture);
        this.AveragePriceHq = float.Parse(lineData[3], CultureInfo.InvariantCulture);
        this.MinPriceNq = float.Parse(lineData[4], CultureInfo.InvariantCulture);
        this.MinPriceHq = float.Parse(lineData[5], CultureInfo.InvariantCulture);
        this.SevenDaySellCount = int.Parse(lineData[6], CultureInfo.InvariantCulture);
        this.LastSellDate = lineData[7] == "" ? null : DateTime.Parse(lineData[7], CultureInfo.InvariantCulture);
        this.LastUpdate = DateTime.Parse(lineData[8], CultureInfo.InvariantCulture);
        this.Available = int.Parse(lineData[9], CultureInfo.InvariantCulture);
    }

    public string[] ToCsv()
    {
        var data = new List<string>()
        {
            this.ItemId.ToString(),
            this.WorldId.ToString(),
            this.AveragePriceNq.ToString(CultureInfo.InvariantCulture),
            this.AveragePriceHq.ToString(CultureInfo.InvariantCulture),
            this.MinPriceNq.ToString(CultureInfo.InvariantCulture),
            this.MinPriceHq.ToString(CultureInfo.InvariantCulture),
            this.SevenDaySellCount.ToString(CultureInfo.InvariantCulture),
            this.LastSellDate.HasValue ? this.LastSellDate.Value.ToString(CultureInfo.InvariantCulture) : "",
            this.LastUpdate.ToString(CultureInfo.InvariantCulture),
            this.Available.ToString(CultureInfo.InvariantCulture)
        };
        return data.ToArray();
    }

    public bool IncludeInCsv()
    {
        return true;
    }

    public static UniversalisPricing FromApi(UniversalisRequestSingle apiResponse, uint worldId)
    {
        UniversalisPricing response = new UniversalisPricing();
        response.AveragePriceNq = apiResponse.averagePriceNQ;
        response.AveragePriceHq = apiResponse.averagePriceHQ;
        response.MinPriceHq = apiResponse.minPriceHQ;
        response.MinPriceNq = apiResponse.minPriceNQ;
        response.ItemId = apiResponse.itemID;
        response.Available = apiResponse.listings?.Length ?? 0;
        response.WorldId = worldId;

        response.listings = apiResponse.listings;
        response.recentHistory = apiResponse.recentHistory;
        int? realMinPriceHq = null;
        int? realMinPriceNq = null;
        if (apiResponse.listings != null && apiResponse.listings.Length != 0)
        {
            foreach (var listing in apiResponse.listings)
            {
                if (listing.hq)
                {
                    if (realMinPriceHq == null || realMinPriceHq > listing.pricePerUnit)
                    {
                        realMinPriceHq = listing.pricePerUnit;
                    }
                }
                else
                {
                    if (realMinPriceNq == null || realMinPriceNq > listing.pricePerUnit)
                    {
                        realMinPriceNq = listing.pricePerUnit;
                    }
                }
            }

            if (realMinPriceHq != null)
            {
                response.MinPriceHq = realMinPriceHq.Value;
            }

            if (realMinPriceNq != null)
            {
                response.MinPriceNq = realMinPriceNq.Value;
            }
        }

        if (apiResponse.recentHistory != null && apiResponse.recentHistory.Length != 0)
        {
            DateTime? latestDate = null;
            var sevenDaySales = 0;
            foreach (var history in apiResponse.recentHistory)
            {
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(history.timestamp).LocalDateTime;
                if (latestDate == null || latestDate <= dateTime)
                {
                    latestDate = dateTime;
                }

                if (dateTime >= DateTime.Now.AddDays(-7))
                {
                    sevenDaySales++;
                }
            }

            response.SevenDaySellCount = sevenDaySales;
            response.LastSellDate = latestDate;
        }
        else
        {
            response.LastSellDate = null;
            response.SevenDaySellCount = 0;
        }

        return response;
    }
}
