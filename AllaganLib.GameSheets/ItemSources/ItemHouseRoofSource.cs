using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemHouseRoofSource : ItemHouseSource
{
    public ItemHouseRoofSource(RowRef<HousingPreset> housingPreset, ItemRow item)
        : base(housingPreset, item, ItemInfoType.HouseRoof)
    {
    }
}