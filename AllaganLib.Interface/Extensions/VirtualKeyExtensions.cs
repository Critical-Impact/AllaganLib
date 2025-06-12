// <copyright file="VirtualKeyExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Dalamud.Game.ClientState.Keys;

namespace AllaganLib.Interface.Extensions;

public static class VirtualKeyExtensions
{
    private static readonly Dictionary<VirtualKey, string> NamedKeys = new()
    {
        { VirtualKey.KEY_0, "0" },
        { VirtualKey.KEY_1, "1" },
        { VirtualKey.KEY_2, "2" },
        { VirtualKey.KEY_3, "3" },
        { VirtualKey.KEY_4, "4" },
        { VirtualKey.KEY_5, "5" },
        { VirtualKey.KEY_6, "6" },
        { VirtualKey.KEY_7, "7" },
        { VirtualKey.KEY_8, "8" },
        { VirtualKey.KEY_9, "9" },
        { VirtualKey.CONTROL, "Ctrl" },
        { VirtualKey.MENU, "Alt" },
        { VirtualKey.SHIFT, "Shift" },
    };

    public static string GetKeyName(this VirtualKey k) => NamedKeys.TryGetValue(k, out var value) ? value : k.ToString();
}