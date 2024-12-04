using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemHouseSource : ItemSource
{
    public RowRef<HousingPreset> HousingPreset { get; }

    public ItemHouseSource(RowRef<HousingPreset> housingPreset, ItemRow item, ItemInfoType infoType)
        : base(infoType)
    {
        this.HousingPreset = housingPreset;
        this.Item = item;
    }

    public override uint Quantity => 1;
}