using System.Numerics;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Helpers;

public static class MapUtility
{
    /// <summary>
    /// Helper method to convert a map marker coordinate to a map coordinate suitable for display to the player.
    /// </summary>
    /// <param name="coord">The coordinate to convert.</param>
    /// <param name="scale">The scale of the map.</param>
    /// <returns>The converted coordinate.</returns>
    public static int MarkerToMap(double coord, double scale)
        => (int)((2 * coord / (scale / 100.0)) + 100.9);

    public static int NodeToMap(double coord, double scale)
        => (int)((2 * coord) + (2048 / (scale / 100.0)) + 100.9);

    public static int IntegerToInternal(int coord, double scale)
        => (int)(coord - 100 - (2048 / (scale / 100.0))) / 2;

    /// <summary>
    /// Helper method to convert one of the game's Vector3 X/Z provided by the game to a map coordinate suitable for
    /// display to the player.
    /// </summary>
    /// <param name="value">The raw float of a game Vector3 X or Z coordinate to convert.</param>
    /// <param name="scale">The scale factor of the map, generally retrieved from Lumina.</param>
    /// <param name="offset">The dimension offset for either X or Z, generally retrieved from Lumina.</param>
    /// <returns>Returns a converted float for display to the player.</returns>
    public static float ConvertWorldCoordXZToMapCoord(float value, uint scale, int offset)
    {
        return (float)((0.019999999552965164 * (double)offset) + (2048.0 / (double)scale) +
                       (0.019999999552965164 * (double)value) + 1.0);
    }

    /// <summary>
    /// Helper method to convert a game Vector3 Y coordinate to a map coordinate suitable for display to the player.
    /// </summary>
    /// <param name="value">The raw float of a game Vector3 Y coordinate to convert.</param>
    /// <param name="zOffset">The zOffset for this map. Retrieved from TerritoryTypeTransient.</param>
    /// <param name="correctZOffset">Optionally enable Z offset correction. When a Z offset of -10,000 is set, replace
    /// it with 0 for calculation purposes to show a more sane Z coordinate.</param>
    /// <returns>Returns a converted float for display to the player.</returns>
    public static float ConvertWorldCoordYToMapCoord(float value, int zOffset, bool correctZOffset = false)
    {
        if ((zOffset == -10000) & correctZOffset)
        {
            zOffset = 0;
        }

        return (float)(((double)value - (double)zOffset) / 100.0);
    }

    /// <summary>
    /// All-in-one helper method to convert a World Coordinate (internal to the game) to a Map Coordinate (visible to
    /// players in the minimap/elsewhere).
    /// </summary>
    /// <remarks>
    /// Note that this method will swap Y and Z in the resulting Vector3 to appropriately reflect the game's display.
    /// </remarks>
    /// <param name="worldCoordinates">A Vector3 of raw World coordinates from the game.</param>
    /// <param name="xOffset">The offset to apply to the incoming X parameter, generally Lumina's Map.OffsetX.</param>
    /// <param name="yOffset">The offset to apply to the incoming Y parameter, generally Lumina's Map.OffsetY.</param>
    /// <param name="zOffset">The offset to apply to the incoming Z parameter, generally Lumina's TerritoryTypeTransient.OffsetZ.</param>
    /// <param name="scale">The global scale to apply to the incoming X and Y parameters, generally Lumina's Map.SizeFactor.</param>
    /// <param name="correctZOffset">An optional mode to "correct" a Z offset of -10000 to be a more human-friendly value.</param>
    /// <returns>Returns a Vector3 representing visible map coordinates.</returns>
    public static Vector3 WorldToMap(
        Vector3 worldCoordinates,
        int xOffset = 0,
        int yOffset = 0,
        int zOffset = 0,
        uint scale = 100,
        bool correctZOffset = false)
    {
        return new Vector3(
            ConvertWorldCoordXZToMapCoord(worldCoordinates.X, scale, xOffset),
            ConvertWorldCoordXZToMapCoord(worldCoordinates.Z, scale, yOffset),
            ConvertWorldCoordYToMapCoord(worldCoordinates.Y, zOffset, correctZOffset));
    }

    /// <summary>
    /// All-in-one helper method to convert a World Coordinate (internal to the game) to a Map Coordinate (visible to
    /// players in the minimap/elsewhere).
    /// </summary>
    /// <remarks>
    /// Note that this method will swap Y and Z to appropriately reflect the game's display.
    /// </remarks>
    /// <param name="worldCoordinates">A Vector3 of raw World coordinates from the game.</param>
    /// <param name="map">A Lumina map to use for offset/scale information.</param>
    /// <param name="territoryTransient">A TerritoryTypeTransient to use for Z offset information.</param>
    /// <param name="correctZOffset">An optional mode to "correct" a Z offset of -10000 to be a more human-friendly value.</param>
    /// <returns>Returns a Vector3 representing visible map coordinates.</returns>
    public static Vector3 WorldToMap(
        Vector3 worldCoordinates,
        Map map,
        TerritoryTypeTransient territoryTransient,
        bool correctZOffset = false)
    {
        return WorldToMap(
            worldCoordinates,
            (int)map.OffsetX,
            (int)map.OffsetY,
            (int)territoryTransient.OffsetZ,
            (uint)map.SizeFactor,
            correctZOffset);
    }

    /// <summary>
    /// All-in-one helper method to convert a World Coordinate (internal to the game) to a Map Coordinate (visible to
    /// players in the minimap/elsewhere).
    /// </summary>
    /// <param name="worldCoordinates">A Vector2 of raw World coordinates from the game.</param>
    /// <param name="xOffset">The offset to apply to the incoming X parameter, generally Lumina's Map.OffsetX.</param>
    /// <param name="yOffset">The offset to apply to the incoming Y parameter, generally Lumina's Map.OffsetY.</param>
    /// <param name="scale">The global scale to apply to the incoming X and Y parameters, generally Lumina's Map.SizeFactor.</param>
    /// <returns>Returns a Vector2 representing visible map coordinates.</returns>
    public static Vector2 WorldToMap(
        Vector2 worldCoordinates,
        int xOffset = 0,
        int yOffset = 0,
        uint scale = 100)
    {
        return new Vector2(
            ConvertWorldCoordXZToMapCoord(worldCoordinates.X, scale, xOffset),
            ConvertWorldCoordXZToMapCoord(worldCoordinates.Y, scale, yOffset));
    }

    /// <summary>
    /// All-in-one helper method to convert a World Coordinate (internal to the game) to a Map Coordinate (visible to
    /// players in the minimap/elsewhere).
    /// </summary>
    /// <param name="worldCoordinates">A Vector2 of raw World coordinates from the game.</param>
    /// <param name="map">A Lumina map to use for offset/scale information.</param>
    /// <returns>Returns a Vector2 representing visible map coordinates.</returns>
    public static Vector2 WorldToMap(Vector2 worldCoordinates, Map map)
    {
        return WorldToMap(worldCoordinates, (int)map.OffsetX, (int)map.OffsetY, (uint)map.SizeFactor);
    }
}