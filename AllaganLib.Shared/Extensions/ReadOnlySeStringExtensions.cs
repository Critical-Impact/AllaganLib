using Dalamud.Utility;
using Lumina.Text.ReadOnly;

namespace AllaganLib.Shared.Extensions;

public static class ReadOnlySeStringExtensions
{
    public static string ToImGuiString(this ReadOnlySeString readOnlySeString)
    {
        return readOnlySeString.ExtractText().StripSoftHyphen();
    }
}