namespace AllaganLib.Universalis.Models;

public class UniversalisRequestSingle
{
    public uint itemID { internal get; set; }

    public float averagePriceNQ { get; set; }

    public float averagePriceHQ { get; set; }

    public float minPriceNQ { get; set; }

    public float minPriceHQ { get; set; }

    public UniversalisHistory[]? recentHistory;
    public UniversalisListing[]? listings;
}
