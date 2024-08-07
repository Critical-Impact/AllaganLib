using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

using AllaganLib.Data.Interfaces;
using CsvHelper;
using Lumina;
using Lumina.Data;

namespace AllaganLib.Data.Service;

/// <summary>
/// A class that saves and loads CSV files.
/// </summary>
public class CsvLoaderService
{
    private readonly GameData gameData;


    /// <summary>
    /// Initializes a new instance of the <see cref="CsvLoaderService"/> class.
    /// </summary>
    /// <param name="gameData">A instance of GameData.</param>
    public CsvLoaderService(GameData gameData)
    {
        this.gameData = gameData;
    }

    /// <summary>
    /// Load a CSV of a given type implementing ICsv.
    /// </summary>
    /// <param name="filePath">The path to the CSV.</param>
    /// <param name="failedLines">A list of any lines that could not be parsed.</param>
    /// <param name="populateData">Should any related models be populated.</param>
    /// <param name="hasHeaders">Does the file being loaded have headers.</param>
    /// <param name="language">When populating data, should this language be used instead of the default.</param>
    /// <typeparam name="T">A type implementing ICsv.</typeparam>
    /// <returns>A list of entries loaded from the csv.</returns>
    public List<T> LoadCsv<T>(
        string filePath,
        out List<string> failedLines,
        bool populateData = true,
        bool hasHeaders = false,
        Language? language = null)
        where T : ICsv, new()
    {
        using var fileStream = new FileStream(filePath, FileMode.Open);
        using (StreamReader reader = new StreamReader(fileStream))
        {
            failedLines = new List<string>();
            var items = new List<T>();

            // Loading an empty file
            if (reader.EndOfStream)
            {
                return items;
            }

            var csvReader = CSVFile.CSVReader.FromString(reader.ReadToEnd());
            var headerSkipped = false;
            foreach (var line in csvReader.Lines())
            {
                if (hasHeaders && !headerSkipped)
                {
                    headerSkipped = true;
                    continue;
                }

                T item = new T();
                try
                {
                    item.FromCsv(line);
                    if (populateData)
                    {
                        item.PopulateData(this.gameData, language ?? this.gameData.Options.DefaultExcelLanguage);
                    }

                    items.Add(item);
                }
                catch (Exception e)
                {
                    failedLines.Add(string.Join(",", line));
                }
            }

            return items;
        }
    }

    /// <summary>
    /// Load a CSV from a embedded resource of a given type implementing ICsv.
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource.</param>
    /// <param name="failedLines">A list of any lines that could not be parsed.</param>
    /// <param name="populateData">Should any related models be populated.</param>
    /// <param name="hasHeaders">Does the file being loaded have headers.</param>
    /// <param name="language">When populating data, should this language be used instead of the default.</param>
    /// <typeparam name="T">A type implementing ICsv.</typeparam>
    /// <returns>A list of entries loaded from the csv.</returns>
    public List<T> LoadResource<T>(
        string resourceName,
        out List<string> failedLines,
        bool populateData = true,
        bool hasHeaders = false,
        Language? language = null)
        where T : ICsv, new()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            failedLines = [];
            if (stream == null)
            {
                return new List<T>();
            }

            using (StreamReader reader = new StreamReader(stream))
            {
                var csvReader = CSVFile.CSVReader.FromString(reader.ReadToEnd());
                var items = new List<T>();
                var headerSkipped = false;
                foreach (var line in csvReader.Lines())
                {
                    if (hasHeaders && !headerSkipped)
                    {
                        headerSkipped = true;
                        continue;
                    }

                    T item = new T();
                    try
                    {
                        item.FromCsv(line);

                        if (populateData)
                        {
                            item.PopulateData(this.gameData, language ?? this.gameData.Options.DefaultExcelLanguage);
                        }

                        items.Add(item);
                    }
                    catch (Exception e)
                    {
                        failedLines.Add(string.Join(",", line));
                    }
                }

                return items;
            }
        }
    }

    /// <summary>
    /// Takes a list of objects implementing ICsv and saves them to a specific file path.
    /// </summary>
    /// <param name="items">A list of objects implementing ICsv.</param>
    /// <param name="filePath">The file path to save to.</param>
    /// <param name="includeHeaders">Should the file being saved include headers.</param>
    /// <typeparam name="T">A type implementing ICsv.</typeparam>
    /// <returns>Returns a value indicating success.</returns>
    public bool ToCsv<T>(List<T> items, string filePath, bool includeHeaders = false)
        where T : ICsv, new()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                var csvWriter = new CSVFile.CSVWriter(writer);
                if (includeHeaders)
                {
                    csvWriter.WriteLine(T.GetHeaders());
                }
                foreach (var line in items)
                {
                    if (line.IncludeInCsv())
                    {
                        csvWriter.WriteLine(line.ToCsv());
                    }
                }

                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }
}
