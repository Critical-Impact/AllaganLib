using AllaganLib.GameSheets.Sheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;

namespace AllaganLib.GameSheets.Sheets.ItemSources;

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


    public override string Name => this.Item.Base.Name.ExtractText();

    public override uint Icon => (uint)this.Cabinet.Base.Category.Value.Icon;
}