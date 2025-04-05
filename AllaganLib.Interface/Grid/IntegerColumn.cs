using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Grid.ColumnFilters;
using AllaganLib.Interface.Services;
using AllaganLib.Shared.Extensions;
using ImGuiNET;
using NaturalSort.Extension;

namespace AllaganLib.Interface.Grid;

public abstract class IntegerColumn<TConfiguration, TData, TMessageBase> : StringFormField<TConfiguration>, IValueColumn<TConfiguration, TData, TMessageBase, string?>
    where TConfiguration : IConfigurable<string?>, INotifyPropertyChanged
{
    private readonly StringColumnFilter stringColumnFilter;

    protected IntegerColumn(ImGuiService imGuiService, StringColumnFilter stringColumnFilter) : base(imGuiService)
    {
        this.stringColumnFilter = stringColumnFilter;
        this.HideReset = true;
        this.ColourModified = false;
    }

    public abstract string? RenderName { get; set; }

    public abstract int Width { get; set; }

    public abstract bool HideFilter { get; set; }

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
                ImGui.TextUnformatted(currentValue.ToString(CultureInfo.InvariantCulture));
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

    public void SetupFilter(IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex)
    {
        this.stringColumnFilter.Setup(column, columnIndex);
    }

    public virtual IEnumerable<TData> Sort(
        TConfiguration configuration,
        IEnumerable<TData> items,
        ImGuiSortDirection direction)
    {
        return direction == ImGuiSortDirection.Ascending ? items.OrderBy(this.CurrentValueAsInteger) : items.OrderByDescending(this.CurrentValueAsInteger);
    }

    public virtual IEnumerable<TData> Filter(TConfiguration config, IEnumerable<TData> items)
    {
        var filterComparisonText = config.Get(this.Key);
        return filterComparisonText == null ? items : items.Where(c =>
        {
            var currentValue = this.CurrentValue(c);
            if (currentValue == null)
            {
                return false;
            }

            if (int.TryParse(currentValue, out var result))
            {
                return result.PassesFilter(filterComparisonText);
            }

            return currentValue.PassesFilter(filterComparisonText);
        });
    }

    public virtual int CurrentValueAsInteger(TData item)
    {
        var currentValue = this.CurrentValue(item);
        if (currentValue == null || !int.TryParse(currentValue, out var result))
        {
            return int.MaxValue;
        }

        return result;
    }


    public virtual string CsvExport(TData item)
    {
        return this.CurrentValue(item)?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public abstract string? CurrentValue(TData item);
}
