namespace AllaganLib.GameSheets.Sheets.Helpers;

public class CompanyCraftMaterial : ISummable<CompanyCraftMaterial>
{
    public CompanyCraftMaterial(uint itemId, uint quantity)
    {
        this.ItemId = itemId;
        this.Quantity = quantity;
    }

    public CompanyCraftMaterial()
    {

    }

    public uint ItemId { get; }
    public uint Quantity { get; }
    public CompanyCraftMaterial Add(CompanyCraftMaterial a, CompanyCraftMaterial b)
    {
        return new CompanyCraftMaterial(a.ItemId, a.Quantity + b.Quantity);
    }
}