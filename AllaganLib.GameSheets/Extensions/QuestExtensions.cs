using AllaganLib.Shared.Extensions;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Extensions;

public static class QuestExtensions
{
    public static string AsConsoleGamesWikiName(this Quest quest)
    {
        var name = quest.Name.ToImGuiString().Replace("#"," ").Replace("  ", " ").Replace(' ', '_');
        name = name.Replace('–', '-');

        if (name.StartsWith("_"))
        {
            name = name.Substring(2);
        }

        return name;
    }
}