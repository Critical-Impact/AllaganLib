using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemHouseWallpaperSource : ItemHouseSource
{
    public ItemHouseWallpaperSource(RowRef<HousingPreset> housingPreset, ItemRow item)
        : base(housingPreset, item, ItemInfoType.HouseWallpaper)
    {
    }
}