using System;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class RecipeLevelTableRow : ExtendedRow<RecipeLevelTable, RecipeLevelTableRow, RecipeLevelTableSheet>
{
    public uint ProgressRequired(Recipe? recipe)
    {
        if (recipe == null)
        {
            return 0;
        }

        return (uint)Math.Floor(this.Base.Difficulty * (recipe.Value.DifficultyFactor / 100.0));
    }

    public uint ProgressRequired(RecipeRow? recipe)
    {
        if (recipe == null)
        {
            return 0;
        }

        return (uint)Math.Floor(this.Base.Difficulty * (recipe.Base.DifficultyFactor / 100.0));
    }
}