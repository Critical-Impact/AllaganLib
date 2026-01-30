using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets.Rows;
using Lumina;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets;

public class LeveSheet : ExtendedSheet<Leve, LeveRow, LeveSheet>, IExtendedSheet
{
    private CraftLeveSheet? craftLeveSheet;
    private GatheringLeveSheet? gatheringLeveSheet;
    private BattleLeveSheet? battleLeveSheet;
    private CompanyLeveSheet? companyLeveSheet;
    private LevelSheet? levelSheet;
    private ENpcResidentSheet? enpcResidentSheet;

    public LeveSheet(GameData gameData, SheetManager sheetManager, SheetIndexer sheetIndexer, ItemInfoCache itemInfoCache)
        : base(gameData, sheetManager, sheetIndexer, itemInfoCache)
    {
    }

    public override void CalculateLookups()
    {
    }

    public CraftLeveSheet GetCraftLeveSheet()
    {
        return this.craftLeveSheet ??=
            this.SheetManager.GetSheet<CraftLeveSheet>();
    }

    public GatheringLeveSheet GetGatheringLeveSheet()
    {
        return this.gatheringLeveSheet ??=
            this.SheetManager.GetSheet<GatheringLeveSheet>();
    }

    public BattleLeveSheet GetBattleLeveSheet()
    {
        return this.battleLeveSheet ??=
            this.SheetManager.GetSheet<BattleLeveSheet>();
    }

    public CompanyLeveSheet GetCompanyLeveSheet()
    {
        return this.companyLeveSheet ??=
            this.SheetManager.GetSheet<CompanyLeveSheet>();
    }

    public LevelSheet GetLevelSheet()
    {
        return this.levelSheet ??=
            this.SheetManager.GetSheet<LevelSheet>();
    }

    public ENpcResidentSheet GetENpcResidentSheet()
    {
        return this.enpcResidentSheet ??=
            this.SheetManager.GetSheet<ENpcResidentSheet>();
    }
}
