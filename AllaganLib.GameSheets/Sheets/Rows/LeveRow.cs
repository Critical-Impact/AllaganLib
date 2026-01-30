using AllaganLib.GameSheets.Model;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class LeveRow : ExtendedRow<Leve, LeveRow, LeveSheet>
{
    private ENpcResidentRow? startENpc;
    private bool startENpcInitialized;

    private CraftLeveRow? craftLeve;
    private bool craftLeveInitialized;

    private GatheringLeveRow? gatheringLeve;
    private bool gatheringLeveInitialized;

    private CompanyLeveRow? companyLeve;
    private bool companyLeveInitialized;

    private BattleLeveRow? battleLeve;
    private bool battleLeveInitialized;

    private RowRef<ParamGrow>? paramGrow;

    public uint ExpReward
    {
        get
        {
            if (this.LeveType == LeveType.Company)
            {
                return (uint)(this.ParamGrow.Value.ScaledQuestXP * (decimal)this.ParamGrow.Value.QuestExpModifier * (decimal)this.Base.ExpFactor);
            }
            return (uint)(this.ParamGrow.Value.ScaledQuestXP * (decimal)this.ParamGrow.Value.QuestExpModifier * (decimal)this.Base.ExpFactor) + 1;
        }
    }

    public uint GilReward
    {
        get
        {
            return this.Base.GilReward;
        }
    }

    public RowRef<ParamGrow> ParamGrow
    {
        get
        {
            return this.paramGrow ??= new RowRef<ParamGrow>(this.Sheet.GameData.Excel, this.Base.ClassJobLevel);
        }
    }

    public ENpcResidentRow? StartENpc
    {
        get
        {
            if (this.startENpcInitialized)
            {
                return this.startENpc;
            }

            this.startENpcInitialized = true;

            if (this.Base.LevelLevemete.IsValid && this.Base.LevelLevemete.RowId != 0)
            {
                var npcId = this.Base.LevelLevemete.Value.Object.RowId;
                this.startENpc = this.Sheet.GetENpcResidentSheet().GetRowOrDefault(npcId);
            }

            return this.startENpc;
        }
    }

    public CraftLeveRow? CraftLeve
    {
        get
        {
            if (this.craftLeveInitialized)
            {
                return this.craftLeve;
            }

            this.craftLeveInitialized = true;

            if (this.LeveType == LeveType.Craft)
            {
                this.craftLeve = this.Sheet
                    .GetCraftLeveSheet()
                    .GetRow(this.Base.DataId.RowId);
            }

            return this.craftLeve;
        }
    }

    public GatheringLeveRow? GatheringLeve
    {
        get
        {
            if (this.gatheringLeveInitialized)
            {
                return this.gatheringLeve;
            }

            this.gatheringLeveInitialized = true;

            if (this.LeveType == LeveType.Gathering)
            {
                this.gatheringLeve = this.Sheet
                    .GetGatheringLeveSheet()
                    .GetRow(this.Base.DataId.RowId);
            }

            return this.gatheringLeve;
        }
    }

    public CompanyLeveRow? CompanyLeve
    {
        get
        {
            if (this.companyLeveInitialized)
            {
                return this.companyLeve;
            }

            this.companyLeveInitialized = true;

            if (this.LeveType == LeveType.Company)
            {
                this.companyLeve = this.Sheet
                    .GetCompanyLeveSheet()
                    .GetRow(this.Base.DataId.RowId);
            }

            return this.companyLeve;
        }
    }

    public BattleLeveRow? BattleLeve
    {
        get
        {
            if (this.battleLeveInitialized)
            {
                return this.battleLeve;
            }

            this.battleLeveInitialized = true;

            if (this.LeveType == LeveType.Battle)
            {
                this.battleLeve = this.Sheet
                    .GetBattleLeveSheet()
                    .GetRow(this.Base.DataId.RowId);
            }

            return this.battleLeve;
        }
    }

    public ILocation? StartLocation
    {
        get
        {
            if (this.LeveType == LeveType.Craft)
            {
                return this.Sheet.GetLevelSheet().GetRow(this.Base.LevelLevemete.RowId);
            }

            if (this.LeveType is LeveType.Battle or LeveType.Gathering or LeveType.Company)
            {
                return this.Sheet.GetLevelSheet().GetRow(this.Base.LevelStart.RowId);
            }

            return null;
        }
    }

    public LeveType LeveType
    {
        get
        {
            if (this.Base.DataId.Is<BattleLeve>())
            {
                return LeveType.Battle;
            }

            if (this.Base.DataId.Is<GatheringLeve>())
            {
                return LeveType.Gathering;
            }

            if (this.Base.DataId.Is<CraftLeve>())
            {
                return LeveType.Craft;
            }

            if (this.Base.DataId.Is<CompanyLeve>())
            {
                return LeveType.Company;
            }

            return LeveType.Unknown;
        }
    }
}
