using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ENpcBaseRow : ExtendedRow<ENpcBase, ENpcBaseRow, ENpcBaseSheet>
{
    private List<ILocation>? locations;
    private RowRef<ENpcResident>? eNpcResident;
    private ENpcResidentRow? residentRow;

    public bool IsVendor => this.Sheet.IsVendor(this.RowId);

    public bool IsHouseVendor => this.Sheet.IsHouseVendor(this.RowId);

    public bool IsHouseVendorChild => this.Sheet.IsHouseVendorChild(this.RowId);

    public bool IsCalamitySalvager => HardcodedItems.CalamitySalvagers.Contains(this.RowId);

    public RowRef<ENpcResident> Resident
    {
        get { return this.eNpcResident ??= new RowRef<ENpcResident>(this.Sheet.GameData.Excel, this.RowId); }
    }


    public ENpcResidentRow ENpcResidentRow
    {
        get { return this.residentRow ??= this.Sheet.GetENpcResidentSheet().GetRow(this.RowId); }
    }

    public IEnumerable<ILocation> Locations
    {
        get { return this.locations ??= this.Sheet.GetLocations(this.RowId)?.Cast<ILocation>().ToList() ?? []; }
    }
}