using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemArmoireSource : ItemSource
{
    public CabinetRow Cabinet { get; }

    public ItemArmoireSource(CabinetRow cabinet, ItemRow item)
        : base(ItemInfoType.Armoire)
    {
        this.Cabinet = cabinet;
        this.Item = item;
    }

    public override uint Quantity => 1;
}