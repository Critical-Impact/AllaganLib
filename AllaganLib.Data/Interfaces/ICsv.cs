using Lumina;
using Lumina.Data;

namespace AllaganLib.Data.Interfaces;

public interface ICsv
{
    /// <summary>
    /// Returns the headers for this CSV.
    /// </summary>
    /// <returns>Returns a list of headers.</returns>
    public static abstract string[] GetHeaders();

    /// <summary>
    /// A function that takes the line data and parses it.
    /// </summary>
    /// <param name="lineData">The line data loaded from the CSV file.</param>
    public void FromCsv(string[] lineData);

    /// <summary>
    /// A function that returns a list of strings representing the object as a string.
    /// </summary>
    /// <returns>A list of strings representing each object.</returns>
    public string[] ToCsv();

    /// <summary>
    /// A function that determines if the given line should be included in any CSV export.
    /// </summary>
    /// <returns>A boolean indicating if the line should be included in the CSV.</returns>
    public bool IncludeInCsv();

    /// <summary>
    /// A function that populates any related LazyRows defined in the class.
    /// </summary>
    /// <param name="gameData">A GameData instance.</param>
    /// <param name="language">The language to retrieve.</param>
    public void PopulateData(GameData gameData, Language language);
}