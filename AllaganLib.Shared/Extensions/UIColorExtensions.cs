using System;
using System.Numerics;
using Lumina.Excel.Sheets;

namespace AllaganLib.Shared.Extensions;

public static class UIColorExtensions
{
    public static Vector4 ConvertUiColorToColor(this UIColor uiColor)
    {
        var temp = BitConverter.GetBytes(uiColor.Dark);
        return new Vector4(
            (float)temp[3] / 255,
            (float)temp[2] / 255,
            (float)temp[1] / 255,
            (float)temp[0] / 255);
    }
}