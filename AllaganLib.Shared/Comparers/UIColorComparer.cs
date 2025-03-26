using System.Collections.Generic;
using Lumina.Excel.Sheets;

namespace AllaganLib.Shared.Comparers;

public class UIColorComparer : IEqualityComparer<UIColor>
{
    public bool Equals(UIColor? x, UIColor? y)
    {
        return x?.Dark == y?.Dark; // based on variable i
    }

    public bool Equals(UIColor x, UIColor y)
    {
        return x.Dark == y.Dark;
    }

    public int GetHashCode(UIColor obj)
    {
        return obj.Dark.GetHashCode(); // hashcode of variable to compare
    }
}