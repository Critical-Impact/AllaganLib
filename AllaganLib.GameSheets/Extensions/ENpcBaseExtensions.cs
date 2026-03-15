using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Extensions;

public static unsafe class ENpcBaseExtensions
{
    extension(ENpcBase row)
    {
        public Collection<uint> ENpcDataRaw => new(row.ExcelPage, parentOffset: row.RowOffset, offset: row.RowOffset, &ENpcDataRawCtor, size: 32);
    }

    private static uint ENpcDataRawCtor(ExcelPage page, uint parentOffset, uint offset, uint i)
        => page.ReadUInt32(offset + (i * 4));
}