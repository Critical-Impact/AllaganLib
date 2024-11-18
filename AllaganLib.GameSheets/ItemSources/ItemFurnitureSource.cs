using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemFurnitureSource : ItemSource
{
    public RowRef<FurnitureCatalogItemList> FurnitureCatalogItem { get; }

    public ItemFurnitureSource(ItemRow item, RowRef<FurnitureCatalogItemList> furnitureCatalogItem)
        : base(ItemInfoType.FurnitureItem)
    {
        this.FurnitureCatalogItem = furnitureCatalogItem;
        this.Item = item;
    }

    public override uint Quantity => 0;
}