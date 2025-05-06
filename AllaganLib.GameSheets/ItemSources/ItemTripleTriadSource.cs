using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.ItemSources;

public class ItemTripleTriadSource : ItemSource
{
    public TripleTriadRow TripleTriadRow { get; }

    public TripleTriadCard? TripleTriadCard => this.Item.Base.AdditionalData.TryGetValue(out TripleTriadCard card) ? card : null;

    public ItemTripleTriadSource(TripleTriadRow tripleTriadRow, ItemRow itemRow)
        : base(ItemInfoType.TripleTriad)
    {
        this.TripleTriadRow = tripleTriadRow;
        this.Item = itemRow;
    }

    public override uint Quantity => 1;
}