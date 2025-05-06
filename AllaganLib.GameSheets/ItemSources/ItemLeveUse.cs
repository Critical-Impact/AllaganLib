using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public abstract class ItemLeveUse : ItemSource
{
    public RowRef<Leve> Leve { get; }
    
    public ItemLeveUse(RowRef<Leve> leve, ItemRow item, ItemInfoType type)
        : base(type)
    {
        this.Leve = leve;
        this.Item = item;
    }
}