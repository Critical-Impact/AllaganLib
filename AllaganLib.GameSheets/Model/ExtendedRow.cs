using Lumina.Excel;

namespace AllaganLib.GameSheets.Model;

public abstract class ExtendedRow<TBase, TExtendedRow, TExtendedSheet>
    where TBase : struct, IExcelRow<TBase>
    where TExtendedRow : ExtendedRow<TBase, TExtendedRow, TExtendedSheet>, new()
    where TExtendedSheet : ExtendedSheet<TBase, TExtendedRow, TExtendedSheet>, IExtendedSheet
{
    public uint RowId { get; set; }

    public ushort? SubRowId { get; set; }

    public TBase Base
    {
        get
        {
            return this.Sheet.BaseSheet.GetRow(this.RowId);
        }
    }

    public RowRef<TBase> RowRef => new(this.Sheet.BaseSheet.Module, this.RowId, this.Sheet.BaseSheet.Language);

    public TExtendedSheet Sheet { get; set; }
}