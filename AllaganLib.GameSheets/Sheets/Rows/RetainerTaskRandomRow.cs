using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class RetainerTaskRandomRow : ExtendedRow<RetainerTaskRandom, RetainerTaskRandomRow, RetainerTaskRandomSheet>
{
    private RetainerTaskRow? retainerTaskRow;
    
    public RetainerTaskRow? RetainerTaskRow
    {
        get
        {
            var retainerTaskId = this.Sheet.GetRetainerTaskByRetainerTaskRandomId(this.RowId);
            if (retainerTaskId != null)
            {
                return this.retainerTaskRow ??= this.Sheet.GetRetainerTaskSheet().GetRow(retainerTaskId.Value);
            }
            
            return null;
        }
    }
    
    private string? formattedName;
    
    public string FormattedName
    {
        get
        {
            if (this.formattedName == null)
            {
                this.formattedName = this.Base.Name.ExtractText() + " - Lv " + (this.RetainerTaskRow?.Base.RetainerLevel.ToString() ?? "Unknown");
            }
            
            return this.formattedName;
        }
    }
    
    private string? nameString;
    
    public string NameString
    {
        get
        {
            if (this.nameString == null)
            {
                this.nameString = this.Base.Name.ExtractText();
            }
            
            return this.nameString;
        }
    }
}