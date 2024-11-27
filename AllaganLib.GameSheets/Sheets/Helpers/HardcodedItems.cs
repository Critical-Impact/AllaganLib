using System.Collections.Generic;

namespace AllaganLib.GameSheets.Sheets.Helpers;

public static class HardcodedItems
{
    public const int GlamourChestSize = 8000;
    public const uint FreeCompanyCreditItemId = 80;
    public const uint FreeCompanyCreditAddonId = 80;
    public const int StormSealId = 20;
    public const int SerpentSealId = 21;
    public const int FlameSealId = 22;
    public const int VentureId = 21072;

    public static readonly uint[] CalamitySalvagers =
    {
        1006004,
        1006005,
        1006006,
        1025913,
        1025914,
        1025915,
        1025916,
        1025917,
        1025918,
        1025919,
        1025920,
        1025921,
        1025922,
        1025923,
        1025924,
    };

    public static readonly uint[] HiddenNodeItemIds =
    {
        7758,  // Grade 1 La Noscean Topsoil
        7761,  // Grade 1 Shroud Topsoil
        7764,  // Grade 1 Thanalan Topsoil
        7759,  // Grade 2 La Noscean Topsoil
        7762,  // Grade 2 Shroud Topsoil
        7765,  // Grade 2 Thanalan Topsoil
        10092, // Black Limestone
        10094, // Little Worm
        10097, // Yafaemi Wildgrass
        12893, // Dark Chestnut
        15865,  // Firelight Seeds
        15866,  // Icelight Seeds
        15867,  // Windlight Seeds
        15868,  // Earthlight Seeds
        15869,  // Levinlight Seeds
        15870,  // Waterlight Seeds
        12534, // Mythrite Ore
        12535, // Hardsilver Ore
        12537, // Titanium Ore
        12579, // Birch Log
        12878, // Cyclops Onion
        12879, // Emerald Beans
    };

    public static readonly Dictionary<uint, uint> GatheringPointBaseToGatheringItem = new()
    {
        {203, 256},  // Grade 1 La Noscean Topsoil, 7758
        {200, 259},// Grade 1 Shroud Topsoil, 7761
        {201, 262},  // Grade 1 Thanalan Topsoil
        {150, 257},  // Grade 2 La Noscean Topsoil
        {209, 260},  // Grade 2 Shroud Topsoil
        {151, 263},  // Grade 2 Thanalan Topsoil
        {210, 289}, // Black Limestone
        {177, 293}, // Little Worm
        {133, 295}, // Yafaemi Wildgrass
        {295, 351}, // Dark Chestnut
        {30, 410},  // Firelight Seeds
        {39, 411},  // Icelight Seeds
        {21, 412},  // Windlight Seeds
        {31, 413},  // Earthlight Seeds
        {25, 414},  // Levinlight Seeds
        {14, 415},  // Waterlight Seeds
        {285, 301}, // Mythrite Ore
        {353, 313}, // Hardsilver Ore
        {286, 307}, // Titanium Ore
        {356, 357}, // Birch Log
        {297, 347}, // Cyclops Onion
        {298, 352}, // Emerald Beans
    };
}