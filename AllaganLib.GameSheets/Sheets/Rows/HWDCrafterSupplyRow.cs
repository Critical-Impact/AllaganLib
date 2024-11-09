using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class HWDCrafterSupplyRow : ExtendedRow<HWDCrafterSupply, HWDCrafterSupplyRow, HWDCrafterSupplySheet>
{
    private Dictionary<uint, HWDCrafterSupply.HWDCrafterSupplyParamsStruct>? supplyItems = null;

    public HWDCrafterSupply.HWDCrafterSupplyParamsStruct? GetSupplyItem(uint itemId)
    {
        this.supplyItems ??= this.Base.HWDCrafterSupplyParams.GroupBy(c => c.ItemTradeIn.RowId)
            .ToDictionary(c => c.Key, c => c.First());

        if (this.supplyItems.TryGetValue(itemId, out var value))
        {
            return value;
        }

        return null;
    }
}