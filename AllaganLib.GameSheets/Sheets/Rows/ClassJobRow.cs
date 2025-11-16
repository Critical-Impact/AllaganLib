using System;
using AllaganLib.GameSheets.Model;
using AllaganLib.Shared.Extensions;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ClassJobRow : ExtendedRow<ClassJob, ClassJobRow, ClassJobSheet>
{
    private ClassJobType? classJobType;

    public int Icon => (int)(62000 + this.RowId);

    public ClassJobType ClassJobType
    {
        get
        {
            if (this.classJobType == null)
            {
                var classAbbreviation = this.Base.Abbreviation.ToImGuiString();
                if (Enum.TryParse(classAbbreviation, out ClassJobType classJobType))
                {
                    this.classJobType = classJobType;
                }
                else
                {
                    this.classJobType = ClassJobType.Unknown;
                }
            }

            return this.classJobType.Value;
        }
    }
}

public enum ClassJobType
{
    Unknown,
    GLA,
    PGL,
    MRD,
    LNC,
    ARC,
    CNJ,
    THM,
    CRP,
    BSM,
    ARM,
    GSM,
    LTW,
    WVR,
    ALC,
    CUL,
    MIN,
    BTN,
    FSH,
    PLD,
    MNK,
    WAR,
    DRG,
    BRD,
    WHM,
    BLM,
    ACN,
    SMN,
    SCH,
    ROG,
    NIN,
    MCH,
    DRK,
    AST,
    SAM,
    RDM,
    BLU,
    GNB,
    DNC,
    RPR,
    SGE,
    VPR,
    PCT
}