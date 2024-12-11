using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ClassJobCategoryRow : ExtendedRow<ClassJobCategory, ClassJobCategoryRow, ClassJobCategorySheet>
{
    private bool lookupsCalculated;
    private bool isGathering;
    private bool isCrafting;
    private bool isCombat;
    private List<uint> classJobIds = new();
    private List<ClassJobRow>? classJobs = null;

    private void CalculateLookups()
    {
        var baseRow = this.Base;
        if (baseRow.MIN || baseRow.BTN || baseRow.FSH)
        {
            this.isGathering = true;
        }

        if (baseRow.CRP || baseRow.WVR || baseRow.BLM || baseRow.ALC || baseRow.ARM || baseRow.BSM || baseRow.CUL ||
            baseRow.GSM || baseRow.LTW)
        {
            this.isCrafting = true;
        }

        if (baseRow.GLA || baseRow.PGL || baseRow.MRD || baseRow.LNC || baseRow.ARC || baseRow.CNJ || baseRow.THM ||
            baseRow.PLD || baseRow.MNK || baseRow.WAR || baseRow.DRG || baseRow.BRD || baseRow.WHM || baseRow.BLM ||
            baseRow.ACN ||
            baseRow.SMN || baseRow.SCH || baseRow.ROG || baseRow.NIN || baseRow.MCH || baseRow.DRK || baseRow.AST ||
            baseRow.SAM || baseRow.RDM || baseRow.BLU || baseRow.GNB || baseRow.DNC || baseRow.RPR || baseRow.SGE ||
            baseRow.VPR || baseRow.PCT)
        {
            this.isCombat = true;
        }

        //Manual at least until we can create custom sheets properly
        this.classJobIds = new List<uint>();

        if (baseRow.GLA)
        {
            this.classJobIds.Add(1);
        }

        if (baseRow.PGL)
        {
            this.classJobIds.Add(2);
        }

        if (baseRow.MRD)
        {
            this.classJobIds.Add(3);
        }

        if (baseRow.LNC)
        {
            this.classJobIds.Add(4);
        }

        if (baseRow.ARC)
        {
            this.classJobIds.Add(5);
        }

        if (baseRow.CNJ)
        {
            this.classJobIds.Add(6);
        }

        if (baseRow.THM)
        {
            this.classJobIds.Add(7);
        }

        if (baseRow.CRP)
        {
            this.classJobIds.Add(8);
        }

        if (baseRow.BSM)
        {
            this.classJobIds.Add(9);
        }

        if (baseRow.ARM)
        {
            this.classJobIds.Add(10);
        }

        if (baseRow.GSM)
        {
            this.classJobIds.Add(11);
        }

        if (baseRow.LTW)
        {
            this.classJobIds.Add(12);
        }

        if (baseRow.WVR)
        {
            this.classJobIds.Add(13);
        }

        if (baseRow.ALC)
        {
            this.classJobIds.Add(14);
        }

        if (baseRow.CUL)
        {
            this.classJobIds.Add(15);
        }

        if (baseRow.MIN)
        {
            this.classJobIds.Add(16);
        }

        if (baseRow.BTN)
        {
            this.classJobIds.Add(17);
        }

        if (baseRow.FSH)
        {
            this.classJobIds.Add(18);
        }

        if (baseRow.PLD)
        {
            this.classJobIds.Add(19);
        }

        if (baseRow.MNK)
        {
            this.classJobIds.Add(20);
        }

        if (baseRow.WAR)
        {
            this.classJobIds.Add(21);
        }

        if (baseRow.DRG)
        {
            this.classJobIds.Add(22);
        }

        if (baseRow.BRD)
        {
            this.classJobIds.Add(23);
        }

        if (baseRow.WHM)
        {
            this.classJobIds.Add(24);
        }

        if (baseRow.BLM)
        {
            this.classJobIds.Add(25);
        }

        if (baseRow.ACN)
        {
            this.classJobIds.Add(26);
        }

        if (baseRow.SMN)
        {
            this.classJobIds.Add(27);
        }

        if (baseRow.SCH)
        {
            this.classJobIds.Add(28);
        }

        if (baseRow.ROG)
        {
            this.classJobIds.Add(29);
        }

        if (baseRow.NIN)
        {
            this.classJobIds.Add(30);
        }

        if (baseRow.MCH)
        {
            this.classJobIds.Add(31);
        }

        if (baseRow.DRK)
        {
            this.classJobIds.Add(32);
        }

        if (baseRow.AST)
        {
            this.classJobIds.Add(33);
        }

        if (baseRow.SAM)
        {
            this.classJobIds.Add(34);
        }

        if (baseRow.RDM)
        {
            this.classJobIds.Add(35);
        }

        if (baseRow.BLU)
        {
            this.classJobIds.Add(36);
        }

        if (baseRow.GNB)
        {
            this.classJobIds.Add(37);
        }

        if (baseRow.DNC)
        {
            this.classJobIds.Add(38);
        }

        if (baseRow.RPR)
        {
            this.classJobIds.Add(39);
        }

        if (baseRow.SGE)
        {
            this.classJobIds.Add(40);
        }

        if (baseRow.VPR)
        {
            this.classJobIds.Add(41);
        }

        if (baseRow.PCT)
        {
            this.classJobIds.Add(42);
        }

        this.lookupsCalculated = true;
    }

    public bool IsGathering
    {
        get
        {
            if (!this.lookupsCalculated)
            {
                this.CalculateLookups();
            }

            return this.isGathering;
        }
    }

    public bool IsCrafting
    {
        get
        {
            if (!this.lookupsCalculated)
            {
                this.CalculateLookups();
            }
            return this.isCrafting;
        }
    }

    public bool IsCombat
    {
        get
        {
            if (!this.lookupsCalculated)
            {
                this.CalculateLookups();
            }
            return this.isCombat;
        }
    }

    public List<uint> ClassJobIds
    {
        get
        {
            if (!this.lookupsCalculated)
            {
                this.CalculateLookups();
            }
            return this.classJobIds;
        }
    }

    public List<ClassJobRow> ClassJobs => this.classJobs ??= this.ClassJobIds.Select(c => this.Sheet.GetClassJobSheet().GetRow(c)).ToList();
}