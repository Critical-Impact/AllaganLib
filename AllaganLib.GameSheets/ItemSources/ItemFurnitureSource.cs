using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemFurnitureSource : ItemSource
{
    public RowRef<HousingFurniture> HousingFurniture { get; }

    public RowRef<FurnitureCatalogItemList> FurnitureCatalogItemList { get; }

    public ItemFurnitureSource(ItemRow item, RowRef<HousingFurniture> housingFurniture, RowRef<FurnitureCatalogItemList> furnitureCatalogItem)
        : base(ItemInfoType.FurnitureItem)
    {
        this.HousingFurniture = housingFurniture;
        this.FurnitureCatalogItemList = furnitureCatalogItem;
        this.Item = item;
    }

    public override uint Quantity => 0;

    public override RelationshipType RelationshipType => RelationshipType.UsedIn;
}