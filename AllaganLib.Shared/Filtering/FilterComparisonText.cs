using System;

namespace AllaganLib.Shared.Extensions;

public class FilterComparisonText
{
    public bool HasOr = false;
    public bool HasAnd = false;
    public bool StartsWithEquals = false;
    public bool StartsWithNegate = false;
    public bool StartsWithFuzzy = false;
    public string SearchText;

    public FilterComparisonText(string filterString)
    {
        this.SearchText = filterString.ToLower().Trim();
        if (filterString.Contains("||", StringComparison.Ordinal))
        {
            this.HasOr = true;
        }

        if (filterString.Contains("&&", StringComparison.Ordinal))
        {
            this.HasAnd = true;
        }

        if (filterString.StartsWith("=", StringComparison.Ordinal) && filterString.Length >= 2)
        {
            this.StartsWithEquals = true;
            this.SearchText = this.SearchText.Substring(1);
        }
        else if (filterString.StartsWith("!", StringComparison.Ordinal) && filterString.Length >= 2)
        {
            this.StartsWithNegate = true;
            this.SearchText = this.SearchText.Substring(1);
        }
        else if (filterString.StartsWith("~", StringComparison.Ordinal) && filterString.Length >= 2)
        {
            this.StartsWithFuzzy = true;
        }
    }
}