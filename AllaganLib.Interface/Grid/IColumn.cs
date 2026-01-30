using System;
using System.Collections.Generic;
using System.ComponentModel;
using AllaganLib.Interface.FormFields;
using AllaganLib.Shared.Extensions;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.Grid;

public interface IColumn<TConfiguration, TData, TMessageBase> : IFormField<TConfiguration>
    where TConfiguration : INotifyPropertyChanged
{
    public string Name { get; set; }

    public string? RenderName { get; set; }

    public int Width { get; set; }

    public bool HideFilter { get; set; }

    public bool IsHidden { get; set; }

    public ImGuiTableColumnFlags ColumnFlags { get; set; }

    public IEnumerable<TMessageBase>? Draw(TConfiguration config, TData item, int rowIndex, int columnIndex);

    public bool DrawFilter(
        TConfiguration configuration,
        IColumn<TConfiguration, TData, TMessageBase> column,
        int columnIndex);

    public List<TMessageBase> DrawFooter(TConfiguration config, List<TData> item, int columnIndex);

    public void SetupFilter(IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex);

    public IEnumerable<TData> Sort(TConfiguration configuration, IEnumerable<TData> items, ImGuiSortDirection direction);

    public IEnumerable<TData> Filter(TConfiguration config, IEnumerable<TData> items);

    public string CsvExport(TData item);

    public void Setup(IRenderTable<TConfiguration, TData, TMessageBase> renderTable, int columnIndex)
    {
        if (this.ColumnFlags.HasFlag(ImGuiTableColumnFlags.WidthFixed))
        {
            ImGui.TableSetupColumn(this.RenderName ?? this.Name, this.ColumnFlags, this.Width, (uint)columnIndex);
        }
        else
        {
            ImGui.TableSetupColumn(this.RenderName ?? this.Name, this.ColumnFlags);
        }
    }
}

public interface IValueColumn<TConfiguration, TData, TMessageBase, TValue> : IColumn<TConfiguration, TData, TMessageBase>
    where TConfiguration : INotifyPropertyChanged
{
    public TValue CurrentValue(TData item);
}