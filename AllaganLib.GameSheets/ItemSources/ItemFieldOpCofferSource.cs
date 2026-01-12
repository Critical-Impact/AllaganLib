using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using LuminaSupplemental.Excel.Model;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemFieldOpCofferSource : ItemSource
{
    public ItemFieldOpCofferSource(ItemRow itemRow, FieldOpCoffer fieldOpCoffer, ItemInfoType infoType)
        : base(infoType)
    {
        this.Item = itemRow;
        this.Quantity = fieldOpCoffer.Min ?? 1;
        this.CofferType = fieldOpCoffer.CofferType;
        this.Probability = fieldOpCoffer.Probability;
        this.Min = fieldOpCoffer.Min;
        this.Max = fieldOpCoffer.Max;
    }

    public uint? Max { get; set; }

    public uint? Min { get; set; }

    public decimal? Probability { get; }

    public FieldOpCofferType CofferType { get; }

    public override uint Quantity { get; }

    public override RelationshipType RelationshipType => RelationshipType.DropsFrom;
}

public class ItemPagosTreasureCofferSource : ItemFieldOpCofferSource
{
    public ItemPagosTreasureCofferSource(ItemRow itemRow, FieldOpCoffer fieldOpCoffer)
        : base(itemRow, fieldOpCoffer, ItemInfoType.PagosTreasure)
    {
    }
}

public class ItemPyrosTreasureCofferSource : ItemFieldOpCofferSource
{
    public ItemPyrosTreasureCofferSource(ItemRow itemRow, FieldOpCoffer fieldOpCoffer)
        : base(itemRow, fieldOpCoffer, ItemInfoType.PyrosTreasure)
    {
    }
}

public class ItemHydatosTreasureCofferSource : ItemFieldOpCofferSource
{
    public ItemHydatosTreasureCofferSource(ItemRow itemRow, FieldOpCoffer fieldOpCoffer)
        : base(itemRow, fieldOpCoffer, ItemInfoType.HydatosTreasure)
    {
    }
}

public class ItemOccultTreasureCofferSource : ItemFieldOpCofferSource
{
    public ItemOccultTreasureCofferSource(ItemRow itemRow, FieldOpCoffer fieldOpCoffer)
        : base(itemRow, fieldOpCoffer, ItemInfoType.OccultTreasure)
    {
    }
}

public class ItemOccultPotSource : ItemFieldOpCofferSource
{
    public ItemOccultPotSource(ItemRow itemRow, FieldOpCoffer fieldOpCoffer)
        : base(itemRow, fieldOpCoffer, ItemInfoType.OccultPot)
    {
    }
}

public class ItemOccultGoldenCofferSource : ItemFieldOpCofferSource
{
    public ItemOccultGoldenCofferSource(ItemRow itemRow, FieldOpCoffer fieldOpCoffer)
        : base(itemRow, fieldOpCoffer, ItemInfoType.OccultGoldenCoffer)
    {
    }
}