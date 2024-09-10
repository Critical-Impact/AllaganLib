using System;
using System.Collections.Generic;
using AllaganLib.Interface.FormFields;
using AllaganLib.Shared.Extensions;
using ImGuiNET;

namespace AllaganLib.Interface.Grid;

public interface IColumn<TConfiguration, TData, TMessageBase> : IFormField<TConfiguration>
{
    public string Name { get; set; }

    public string? RenderName { get; set; }

    public int Width { get; set; }

    public bool HideFilter { get; set; }

    public ImGuiTableColumnFlags ColumnFlags { get; set; }

    public IEnumerable<TMessageBase>? Draw(TConfiguration config, TData item, int rowIndex, int columnIndex);

    public bool DrawFilter(
        TConfiguration configuration,
        IColumn<TConfiguration, TData, TMessageBase> column,
        int columnIndex);

    public void SetupFilter(IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex);

    public IEnumerable<TData> Sort(TConfiguration configuration, IEnumerable<TData> items, ImGuiSortDirection direction);

    public IEnumerable<TData> Filter(TConfiguration config, IEnumerable<TData> items);

    public string CsvExport(TData item);

    public void Setup(int columnIndex)
    {
        ImGui.TableSetupColumn(this.RenderName ?? this.Name, this.ColumnFlags, this.Width, (uint)columnIndex);
    }
}

public interface IValueColumn<TConfiguration, TData, TMessageBase, TValue> : IColumn<TConfiguration, TData, TMessageBase>
{
    public TValue CurrentValue(TData item);
}