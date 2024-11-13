using AllaganLib.GameSheets.Model;
using AllaganLib.Shared.Time;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class GatheringPointTransientRow : ExtendedRow<GatheringPointTransient, GatheringPointTransientRow, GatheringPointTransientSheet>
{
    private BitfieldUptime? uptime;
    private bool uptimeCalculated = false;
    
    public bool TimedNode => this.Base.GatheringRarePopTimeTable.RowId != 0;
    
    public bool EphemeralNode => this.Base.EphemeralStartTime < 65535 && (this.Base.EphemeralStartTime != 0 || this.Base.EphemeralEndTime != 0);
    
    public BitfieldUptime? GetGatheringUptime()
    {
        if ((!this.TimedNode && !this.EphemeralNode) || this.uptimeCalculated)
        {
            return this.uptime;
        }
        
        // Check for ephemeral nodes
        if (this.Base.GatheringRarePopTimeTable.RowId == 0)
        {
            var time = new BitfieldUptime(this.Base.EphemeralStartTime, this.Base.EphemeralEndTime);
            this.uptime = time;
        }
        else
        {
            this.uptime = new BitfieldUptime(this.Base.GatheringRarePopTimeTable.Value);
        }
        
        return this.uptime;
    }
}