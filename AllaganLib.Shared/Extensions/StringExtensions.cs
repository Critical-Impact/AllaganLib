using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AllaganLib.Shared.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string npcNameSingular)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(npcNameSingular.ToLower());
    }

    public static void OpenBrowser(this string url) {
        Process.Start(new ProcessStartInfo {FileName = url, UseShellExecute = true});
    }

    public static string ToParseable(this string input)
    {
        if (input.Contains("\u00a0"))
        {
            var startIndex = input.IndexOf("\u00a0");
            input = input.Remove(startIndex);
        }

        return input.ToLower().RemoveWhitespace().RemoveSpecialCharacters();
    }

    public static string RemoveWhitespace(this string input)
    {
        return new string(
            input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
    }

    public static string RemoveSpecialCharacters(this string input)
    {
        var rgx = new Regex("[^a-zA-Z0-9]");
        return rgx.Replace(input, "");
    }
}