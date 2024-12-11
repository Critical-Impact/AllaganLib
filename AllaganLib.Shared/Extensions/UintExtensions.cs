using System;
using System.Numerics;

namespace AllaganLib.Shared.Extensions;

public static class UintExtensions
{
    public static Vector4 UiColorToVector4Color(this int color)
    {
        var temp = BitConverter.GetBytes(color);
        return new Vector4(
            (float)temp[3] / 255,
            (float)temp[2] / 255,
            (float)temp[1] / 255,
            (float)temp[0] / 255);
    }

    public static Vector4 ThreeChannelColorToVector4Color(this uint color)
    {
        var temp = BitConverter.GetBytes(color);
        return new Vector4(
            (float)temp[2] / 255,
            (float)temp[1] / 255,
            (float)temp[0] / 255,
            1.0f);
    }
}