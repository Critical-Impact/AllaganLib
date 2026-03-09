using System;
using AllaganLib.GameSheets.Model;
using AllaganLib.Shared.Extensions;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ClassJobRow : ExtendedRow<ClassJob, ClassJobRow, ClassJobSheet>
{
    private ClassJobType? classJobType;
    private ClassJobCategoryRow? classJobCategoryRow;

    public int Icon => (int)(62000 + this.RowId);

    public RoleType Role
    {
        get
        {
            if (this.Base.Role == 0)
            {
                var category = this.ClassJobCategory;
                if (category != null)
                {
                    if (category.IsCrafting)
                    {
                        return RoleType.Crafting;
                    }

                    if (category.IsGathering)
                    {
                        return RoleType.Gathering;
                    }

                    return RoleType.Other;
                }
            }

            return (RoleType)this.Base.Role;
        }
    }

    public ClassJobCategoryRow? ClassJobCategory
    {
        get
        {
            return this.classJobCategoryRow ??= this.Sheet.GetClassCategorySheet().GetRowOrDefault(this.Base.ClassJobCategory.RowId);
        }
    }

    public ClassJobType ClassJobType
    {
        get
        {
            if (this.classJobType == null)
            {
                this.classJobType = (ClassJobType)this.Base.RowId;
            }

            return this.classJobType.Value;
        }
    }
}

public enum RoleType
{
    Tank = 1,
    DPSMelee = 2,
    DPSRanged = 3,
    Healer = 4,
    Crafting = 5,
    Gathering = 6,
    Other = 7
}

public enum ClassJobType
{
    Unknown = 0,
    GLA = 1,
    PGL = 2,
    MRD = 3,
    LNC = 4,
    ARC = 5,
    CNJ = 6,
    THM = 7,
    CRP = 8,
    BSM = 9,
    ARM = 10,
    GSM = 11,
    LTW = 12,
    WVR = 13,
    ALC = 14,
    CUL = 15,
    MIN = 16,
    BTN = 17,
    FSH = 18,
    PLD = 19,
    MNK = 20,
    WAR = 21,
    DRG = 22,
    BRD = 23,
    WHM = 24,
    BLM = 25,
    ACN = 26,
    SMN = 27,
    SCH = 28,
    ROG = 29,
    NIN = 30,
    MCH = 31,
    DRK = 32,
    AST = 33,
    SAM = 34,
    RDM = 35,
    BLU = 36,
    GNB = 37,
    DNC = 38,
    RPR = 39,
    SGE = 40,
    VPR = 41,
    PCT = 42,
}