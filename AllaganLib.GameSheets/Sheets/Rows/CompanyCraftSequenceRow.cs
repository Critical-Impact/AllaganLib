using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Helpers;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class
    CompanyCraftSequenceRow : ExtendedRow<CompanyCraftSequence, CompanyCraftSequenceRow, CompanyCraftSequenceSheet>
{
    private CompanyCraftPartRow[]? activeCompanyCraftParts;
    private Dictionary<uint, List<CompanyCraftMaterial>>? partsRequired;
    private List<CompanyCraftMaterial>? allPartsRequired;

    public CompanyCraftPartRow[] CompanyCraftParts
    {
        get
        {
            if (this.activeCompanyCraftParts == null)
            {
                var companyCraftPartSheet = this.Sheet.GetCompanyCraftPartSheet();
                this.activeCompanyCraftParts = this.Base.CompanyCraftPart
                    .Where(c => c.RowId != 0)
                    .Select(c => companyCraftPartSheet.GetRow(c.RowId))
                    .ToArray();
            }

            return this.activeCompanyCraftParts;
        }
    }

    public List<CompanyCraftMaterial> MaterialsRequired(uint? phase)
    {
        if (this.partsRequired == null || this.allPartsRequired == null)
        {
            this.partsRequired = new Dictionary<uint, List<CompanyCraftMaterial>>();
            this.allPartsRequired = new List<CompanyCraftMaterial>();

            uint totalIndex = 0;

            foreach (var part in this.CompanyCraftParts)
            {
                foreach (var process in part.CompanyCraftProcess)
                {
                    for (var index = 0; index < process.Base.SupplyItem.Count; index++)
                    {
                        var supplyItem = process.Base.SupplyItem[index];
                        var setsRequired = process.Base.SetsRequired[index];
                        var setQuantity = process.Base.SetQuantity[index];

                        var actualItem = supplyItem.Value;
                        if (actualItem.Item.RowId != 0 && actualItem.Item.ValueNullable != null)
                        {
                            var material = new CompanyCraftMaterial(
                                actualItem.Item.RowId,
                                (uint)setsRequired * setQuantity);

                            if (!this.partsRequired.ContainsKey(totalIndex))
                            {
                                this.partsRequired[totalIndex] = new List<CompanyCraftMaterial>();
                            }

                            this.partsRequired[totalIndex].Add(material);
                            this.allPartsRequired.Add(material);
                        }
                    }
                }

                totalIndex++;
            }

            this.allPartsRequired = this.allPartsRequired.GroupBy(c => c.ItemId)
                .Select(c => c.Aggregate((a, b) => a.Add(a, b))).ToList();
        }

        if (phase == null)
        {
            return this.allPartsRequired;
        }

        return this.partsRequired.TryGetValue(phase.Value, out List<CompanyCraftMaterial>? materials)
            ? materials
            : new List<CompanyCraftMaterial>();
    }
}