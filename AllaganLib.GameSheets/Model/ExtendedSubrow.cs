using Lumina.Excel;

namespace AllaganLib.GameSheets.Model;

public abstract class ExtendedSubrow<TBase, TExtendedRow, TExtendedSheet>
    where TBase : struct, IExcelSubrow<TBase>
    where TExtendedRow : ExtendedSubrow<TBase, TExtendedRow, TExtendedSheet>, new()
    where TExtendedSheet : ExtendedSubrowSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
{
    public uint RowId { get; set; }

    public ushort SubRowId { get; set; }

    public TBase Base
    {
        get
        {
            return this.Sheet.BaseSheet.GetSubrow(this.RowId, this.SubRowId);
        }
    }

    public TExtendedSheet Sheet { get; set; }
}