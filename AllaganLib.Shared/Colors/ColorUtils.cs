using System;
using System.Globalization;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using Lumina.Excel.Sheets;

namespace AllaganLib.Shared.Colors;

public static class ColorUtils
{
    public static Vector4 ConvertUiColorToColor(UIColor uiColor)
    {
        return ConvertUiColorToColor(uiColor.Dark);
    }

    public static Vector4 ConvertUiColorToColor(uint color)
    {
        var temp = BitConverter.GetBytes(color);
        return new Vector4((float) temp[3] / 255,
            (float) temp[2] / 255,
            (float) temp[1] / 255,
            (float) temp[0] / 255);
    }

    public static Vector4 Convert3ChannelUintToColorVector4(uint color)
    {
        var temp = BitConverter.GetBytes(color);
        return new Vector4((float) temp[2] / 255,
            (float) temp[1] / 255,
            (float) temp[0] / 255,
            1.0f);
    }

    public static ByteColor ColorFromHex(string hexString, int alpha)
    {
        if (hexString.IndexOf('#') != -1)
            hexString = hexString.Replace("#", "");

        var r = int.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
        var g = int.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
        var b = int.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

        return new ByteColor() {R = (byte) r, B = (byte) b, G = (byte) g, A = (byte) alpha};
    }

    public static string ColorToHex(ByteColor color, bool includeHash = true)
    {
        string hex = $"{color.R:X2}{color.G:X2}{color.B:X2}";
        return includeHash ? $"#{hex}" : hex;
    }

    public static ByteColor ColorFromVector4(Vector4 hexString)
    {
        return new () {R = (byte) (hexString.X * 0xFF), B = (byte) (hexString.Z * 0xFF), G = (byte) (hexString.Y * 0xFF), A = (byte) (hexString.W * 0xFF)};
    }
}