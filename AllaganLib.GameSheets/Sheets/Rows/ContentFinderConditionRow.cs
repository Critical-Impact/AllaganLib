using System.Collections.Generic;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class ContentFinderConditionRow : ExtendedRow<ContentFinderCondition, ContentFinderConditionRow,
    ContentFinderConditionSheet>
{
    private string? formattedName;
    private ClassJobCategoryRow? acceptClassJobCategory;
    private string? roulettes;

    public string FormattedName => this.formattedName ??= this.Base.Name.ExtractText().ToTitleCase();

    public ClassJobCategoryRow? AcceptClassJobCategory
    {
        get
        {
            if (this.acceptClassJobCategory == null && this.Base.AcceptClassJobCategory.IsValid)
            {
                var classJobCategorySheet = this.Sheet.GetClassJobCategorySheet();
                this.acceptClassJobCategory = classJobCategorySheet.GetRowOrDefault(this.Base.AcceptClassJobCategory.RowId);
            }

            return this.acceptClassJobCategory;
        }
    }

    public string Roulettes
    {
        get
        {
            if (this.roulettes != null)
            {
                return this.roulettes;
            }

            var roulettesList = new List<string>();
            var contentRouletteSheet = this.Sheet.GetContentRouletteSheet();

            if (this.Base.LevelingRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(1).Base.Category.ExtractText());
            }

            if (this.Base.HighLevelRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(2).Base.Category.ExtractText());
            }

            if (this.Base.MSQRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(3).Base.Category.ExtractText());
            }

            if (this.Base.GuildHestRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(4).Base.Category.ExtractText());
            }

            if (this.Base.ExpertRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(5).Base.Category.ExtractText());
            }

            if (this.Base.TrialRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(6).Base.Category.ExtractText());
            }

            if (this.Base.DailyFrontlineChallenge)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(7).Base.Category.ExtractText());
            }

            if (this.Base.LevelCapRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(8).Base.Category.ExtractText());
            }

            if (this.Base.MentorRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(9).Base.Category.ExtractText());
            }

            if (this.Base.AllianceRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(15).Base.Category.ExtractText());
            }

            if (this.Base.NormalRaidRoulette)
            {
                roulettesList.Add(contentRouletteSheet.GetRow(17).Base.Category.ExtractText());
            }

            this.roulettes = string.Join(", ", roulettesList);
            return this.roulettes;
        }
    }
}