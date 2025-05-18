using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Grid.ColumnFilters;
using AllaganLib.Interface.Services;
using AllaganLib.Shared.Extensions;
using ImGuiNET;
using NaturalSort.Extension;

namespace AllaganLib.Interface.Grid;

public abstract class StringColumn<TConfiguration, TData, TMessageBase> : StringFormField<TConfiguration>, IValueColumn<TConfiguration, TData, TMessageBase, string?>
    where TConfiguration : IConfigurable<string?>, INotifyPropertyChanged
{
    private readonly StringColumnFilter stringColumnFilter;

    protected StringColumn(ImGuiService imGuiService, StringColumnFilter stringColumnFilter) : base(imGuiService)
    {
        this.stringColumnFilter = stringColumnFilter;
        this.HideReset = true;
        this.ColourModified = false;
    }

    public abstract string? RenderName { get; set; }

    public abstract int Width { get; set; }

    public abstract bool HideFilter { get; set; }

    public virtual bool IsHidden { get; set; }

    public abstract ImGuiTableColumnFlags ColumnFlags { get; set; }

    public abstract string EmptyText { get; set; }

    public virtual IEnumerable<TMessageBase>? Draw(TConfiguration config, TData item, int rowIndex, int columnIndex)
    {
        ImGui.TableNextColumn();
        if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
        {
            var currentValue = this.CurrentValue(item);
            if (currentValue != null)
            {
                ImGui.AlignTextToFramePadding();
                ImGui.TextUnformatted(currentValue);
            }
            else
            {
                ImGui.TextUnformatted(this.EmptyText);
            }
        }

        return null;
    }

    public bool DrawFilter(TConfiguration configuration, IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex)
    {
        return this.stringColumnFilter.Draw(configuration, column, columnIndex);
    }

    public virtual List<TMessageBase> DrawFooter(TConfiguration config, List<TData> item, int columnIndex)
    {
        ImGui.TableNextColumn();
        return new List<TMessageBase>();
    }

    public void SetupFilter(IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex)
    {
        this.stringColumnFilter.Setup(column, columnIndex);
    }

    public virtual IEnumerable<TData> Sort(
        TConfiguration configuration,
        IEnumerable<TData> items,
        ImGuiSortDirection direction)
    {
        return direction == ImGuiSortDirection.Ascending ? items.OrderBy(this.CurrentValue, StringComparison.OrdinalIgnoreCase.WithNaturalSort()) : items.OrderByDescending(this.CurrentValue, StringComparison.OrdinalIgnoreCase.WithNaturalSort());
    }

    public virtual IEnumerable<TData> Filter(TConfiguration config, IEnumerable<TData> items)
    {
        var filterComparisonText = config.Get(this.Key) ?? string.Empty;
        return filterComparisonText == string.Empty ? items : items.Where(c =>
        {
            var currentValue = this.CurrentValue(c);
            if (currentValue == null)
            {
                return false;
            }

            return currentValue.ToLower().PassesFilter(filterComparisonText);
        });
    }

    public virtual string CsvExport(TData item)
    {
        return this.CurrentValue(item) ?? string.Empty;
    }

    public abstract string? CurrentValue(TData item);
}
