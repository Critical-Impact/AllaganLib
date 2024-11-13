using System.Globalization;

namespace AllaganLib.GameSheets.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string npcNameSingular)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(npcNameSingular.ToLower());
    }
}