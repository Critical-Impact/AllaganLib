namespace AllaganLib.Interface.Grid;

public struct RenderTableCsvExportOptions
{
    public RenderTableCsvExportOptions(string exportPath)
    {
        this.ExportPath = exportPath;
    }

    public string ExportPath { get; set; }

    public bool IncludeHeaders { get; set; }

    public bool UseFiltering { get; set; }
}