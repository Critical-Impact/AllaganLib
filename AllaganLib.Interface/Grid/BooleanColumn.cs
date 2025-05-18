using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Grid.ColumnFilters;
using AllaganLib.Interface.Services;
using AllaganLib.Shared.Extensions;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using NaturalSort.Extension;

namespace AllaganLib.Interface.Grid;

public abstract class BooleanColumn<TConfiguration, TData, TMessageBase> : StringFormField<TConfiguration>, IValueColumn<TConfiguration, TData, TMessageBase, string?>
    where TConfiguration : IConfigurable<string?>, INotifyPropertyChanged
{
    private readonly ChoiceColumnFilter choiceColumnFilter;

    protected BooleanColumn(ImGuiService imGuiService, ChoiceColumnFilter? choiceColumnFilter = null)
        : base(imGuiService)
    {
        this.choiceColumnFilter = choiceColumnFilter ?? new ChoiceColumnFilter(new List<string>() { "true", "false" });
        this.HideReset = true;
        this.ColourModified = false;
    }

    public abstract string? RenderName { get; set; }

    public abstract int Width { get; set; }

    public bool HideFilter { get; set; }

    public virtual bool IsHidden { get; set; }

    public virtual ImGuiTableColumnFlags ColumnFlags { get; set; }

    public virtual IEnumerable<TMessageBase>? Draw(TConfiguration config, TData item, int rowIndex, int columnIndex)
    {
        ImGui.TableNextColumn();
        if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
        {
            var currentValue = this.CurrentValue(item);
            if (currentValue != null)
            {
                var boolValue = currentValue == "true";
                using var disabled = ImRaii.Disabled(true);
                ImGui.Checkbox("##checkBox" + this.Key, ref boolValue);
            }
        }

        return null;
    }

    public bool DrawFilter(TConfiguration configuration, IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex)
    {
        return this.choiceColumnFilter.Draw(configuration, column, columnIndex);
    }

    public virtual List<TMessageBase> DrawFooter(TConfiguration config, List<TData> item, int columnIndex)
    {
        ImGui.TableNextColumn();
        return new List<TMessageBase>();
    }

    public void SetupFilter(IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex)
    {
        this.choiceColumnFilter.Setup(column, columnIndex);
    }

    public virtual IEnumerable<TData> Sort(
        TConfiguration configuration,
        IEnumerable<TData> items,
        ImGuiSortDirection direction)
    {
        return direction == ImGuiSortDirection.Ascending ? items.OrderBy(this.CurrentValue) : items.OrderByDescending(this.CurrentValue);
    }

    public virtual IEnumerable<TData> Filter(TConfiguration config, IEnumerable<TData> items)
    {
        var filterValue = config.Get(this.Key);
        if (filterValue == null)
        {
            return items;
        }

        return items.Where(
            c =>
        {
            var currentValue = this.CurrentValue(c);
            if (currentValue == null)
            {
                return false;
            }

            if (filterValue == "true")
            {
                return currentValue == "true";
            }

            return currentValue == "false";
        });
    }

    public abstract string? CurrentValue(TData item);

    public virtual string CsvExport(TData item)
    {
        return this.CurrentValue(item) ?? string.Empty;
    }
}