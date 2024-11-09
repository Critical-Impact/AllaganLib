using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class CompanyCraftPartRow : ExtendedRow<CompanyCraftPart, CompanyCraftPartRow, CompanyCraftPartSheet>
{
    private List<CompanyCraftProcessRow>? companyCraftProcess;

    public List<CompanyCraftProcessRow> CompanyCraftProcess
    {
        get
        {
            return this.companyCraftProcess ??= this.Base.CompanyCraftProcess
                .Select(c => this.Sheet.GetCompanyCraftProcessSheet().GetRow(c.RowId)).ToList();
        }
    }
}