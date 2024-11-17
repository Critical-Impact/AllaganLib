using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class RetainerTaskNormalRow : ExtendedRow<RetainerTaskNormal, RetainerTaskNormalRow, RetainerTaskNormalSheet>
{
    private RetainerTaskRow? retainerTaskRow;
    
    public RetainerTaskRow? RetainerTaskRow
    {
        get
        {
            var retainerTaskId = this.Sheet.GetRetainerTaskByRetainerTaskNormalId(this.RowId);
            if (retainerTaskId != null)
            {
                return this.retainerTaskRow ??= this.Sheet.GetRetainerTaskSheet().GetRow(retainerTaskId.Value);
            }
            
            return null;
        }
    }
    
    public string TaskName
    {
        get
        {
            if (this.RetainerTaskRow != null)
            {
                var classJobName = this.RetainerTaskRow.ClassJobCategoryRow?.Base.Name.ToString();
                var level = this.RetainerTaskRow.Base.RetainerLevel;
                return classJobName + " - Lv " + level;
            }
            
            return "Unknown";
        }
    }
    
    public ushort TaskTime
    {
        get
        {
            if (this.RetainerTaskRow != null)
            {
                return this.RetainerTaskRow.Base.MaxTimemin;
            }
            
            return 0;
        }
    }
    
    public string Quantities
    {
        get
        {
            return string.Join(", ", this.Base.Quantity.Where(c => c != 0).Select(c => c.ToString()));
        }
    }
}