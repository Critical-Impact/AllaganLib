namespace AllaganLib.Universalis.Models;

public class UniversalisListing
{
    public int lastReviewTime { get; set; }

    public int pricePerUnit { get; set; }

    public int quantity { get; set; }

    public int stainID { get; set; }

    public string creatorName { get; set; }

    public object creatorID { get; set; }

    public bool hq { get; set; }

    public bool isCrafted { get; set; }

    public object listingID { get; set; }

    public object[] materia { get; set; }

    public bool onMannequin { get; set; }

    public int retainerCity { get; set; }

    public string retainerID { get; set; }

    public string retainerName { get; set; }

    public string sellerID { get; set; }

    public int total { get; set; }
}
