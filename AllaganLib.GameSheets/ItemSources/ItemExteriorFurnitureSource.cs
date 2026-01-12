using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemExteriorFurnitureSource : ItemSource
{
    public RowRef<HousingYardObject> HousingYardObject { get; }

    public ItemExteriorFurnitureSource(ItemRow item, RowRef<HousingYardObject> housingYardObject)
        : base(ItemInfoType.ExteriorFurnitureItem)
    {
        this.HousingYardObject = housingYardObject;
        this.Item = item;
    }

    public override uint Quantity => 0;

    public override RelationshipType RelationshipType => RelationshipType.UsedIn;
}