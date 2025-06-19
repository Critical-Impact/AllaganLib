using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using AllaganLib.Data.Service;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs;
using ImGuiNET;
using Lumina.Data.Parsing.Scd;

namespace AllaganLib.Interface.Grid;

public abstract class RenderTable<TConfiguration, TData, TMessageBase> : IDisposable, IRenderTable<TConfiguration, TData, TMessageBase>
    where TConfiguration : INotifyPropertyChanged
{
    private readonly CsvLoaderService csvLoaderService;
    private Task? currentLoadTask;
    private CancellationTokenSource? cancellationTokenSource;

    public RenderTable(
        CsvLoaderService csvLoaderService,
        TConfiguration searchFilter,
        List<IColumn<TConfiguration, TData, TMessageBase>> columns,
        ImGuiTableFlags tableFlags,
        string name,
        string key)
    {
        this.csvLoaderService = csvLoaderService;
        this.SearchFilter = searchFilter;
        this.Columns = columns;
        this.TableFlags = tableFlags;
        this.Name = name;
        this.Key = key;
        this.SearchFilter.PropertyChanged += this.SearchFilterModified;
    }

    private void SearchFilterModified(object? sender, PropertyChangedEventArgs e)
    {
        this.IsDirty = true;
    }

    public TConfiguration SearchFilter { get; }

    public virtual List<IColumn<TConfiguration, TData, TMessageBase>> Columns { get; set; }

    public virtual ImGuiTableFlags TableFlags { get; set; }

    public virtual string Name { get; set; }

    public virtual string Key { get; set; }

    public virtual int? FreezeCols { get; set; }

    public virtual int? FreezeRows { get; set; }

    public int? SortColumn { get; set; }

    public bool ShowFilterRow { get; set; }

    public bool HasFooter { get; set; }

    public bool UseClipper { get; set; } = true;

    public virtual Func<TData, List<TMessageBase>>? RightClickFunc { get; set; } = null;

    public ImGuiSortDirection? SortDirection { get; set; }

    public bool IsDirty { get; set; } = true;

    private List<TData> items = new();

    public abstract List<TData> GetItems();

    public virtual Task<List<TData>> GetItemsAsync()
    {
        // Default implementation wraps the synchronous method
        return Task.FromResult(this.GetItems());
    }

    public bool IsLoading => this.currentLoadTask is { IsCompleted: false };

    public List<TData> GetFilteredItems(TConfiguration configuration, bool isDirty = false)
    {
        if (this.IsDirty || isDirty)
        {
            var newData = this.GetItems().AsEnumerable();
            for (var index = 0; index < this.Columns.Count; index++)
            {
                var column = this.Columns[index];
                if (column.IsHidden)
                {
                    continue;
                }
                newData = column.Filter(configuration, newData);
                if (this.SortColumn == index)
                {
                    newData = column.Sort(configuration, newData, this.SortDirection ?? ImGuiSortDirection.Ascending);
                }
            }

            this.items = newData.ToList();
            this.IsDirty = false;
        }

        return this.items;
    }

    public async Task<List<TData>> GetFilteredItemsAsync(TConfiguration configuration)
    {
        if (this.IsDirty)
        {
            if (this.cancellationTokenSource != null)
            {
                await this.cancellationTokenSource.CancelAsync();
            }

            this.cancellationTokenSource = new CancellationTokenSource();
            var token = this.cancellationTokenSource.Token;
            this.IsDirty = false;

            var task = Task.Run(() => this.GetFilteredItems(configuration, true), token);
            this.currentLoadTask = task;
            await this.currentLoadTask;
            if (!task.IsCanceled)
            {
                this.items = task.Result;
            }
        }

        return this.items;
    }


    public void SaveToCsv(TConfiguration configuration, RenderTableCsvExportOptions exportOptions)
    {
        var lines = exportOptions.UseFiltering ? this.GetFilteredItems(configuration) : this.GetItems();
        List<List<string>> rows = new();
        if (exportOptions.IncludeHeaders)
        {
            var headers = this.Columns.Where(c => !c.IsHidden).Select(column => column.RenderName ?? column.Name).ToList();
            rows.Add(headers);
        }

        foreach (var line in lines)
        {
            var newRow = this.Columns.Where(c => !c.IsHidden).Select(column => column.CsvExport(line)).ToList();
            rows.Add(newRow);
        }

        this.csvLoaderService.ToCsv(rows, exportOptions.ExportPath);
    }

    private readonly Dictionary<int, float> ColumnWidths = new();

    public virtual unsafe List<TMessageBase> Draw(TConfiguration configuration, Vector2 size, bool shouldDraw = true)
    {
        var messages = new List<TMessageBase>();

        if (this.Columns.Count == 0 || !shouldDraw)
        {
            ImGui.Text("No columns have been added to the table.");
            return messages;
        }

        using (var filterTableChild = ImRaii.Child(
                   "FilterTableContent",
                   size * ImGui.GetIO().FontGlobalScale,
                   false,
                   ImGuiWindowFlags.HorizontalScrollbar))
        {
            if (filterTableChild.Success)
            {
                using var table = ImRaii.Table(this.Key, this.Columns.Count(c => !c.IsHidden), this.TableFlags);
                if (table.Success)
                {
                    var refresh = false;

                    for (var index = 0; index < this.Columns.Count; index++)
                    {
                        var column = this.Columns[index];
                        if (column.IsHidden)
                        {
                            continue;
                        }
                        column.Setup(this, index);
                    }

                    ImGui.TableSetupScrollFreeze(Math.Min(this.FreezeCols ?? 0, this.Columns.Count(c => !c.IsHidden)), this.FreezeRows ?? 0);

                    ImGui.TableHeadersRow();


                    var currentSortSpecs = ImGui.TableGetSortSpecs();

                    if (currentSortSpecs.NativePtr != null && currentSortSpecs.SpecsDirty)
                    {
                        var actualSpecs = currentSortSpecs.Specs;
                        if (this.SortColumn != actualSpecs.ColumnIndex)
                        {
                            this.SortColumn = actualSpecs.ColumnIndex;
                            refresh = true;
                        }

                        if (this.SortDirection != actualSpecs.SortDirection)
                        {
                            this.SortDirection = actualSpecs.SortDirection;
                            refresh = true;
                        }

                        currentSortSpecs.SpecsDirty = false;
                    }

                    if (this.ShowFilterRow)
                    {
                        ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
                        for (var index = 0; index < this.Columns.Count; index++)
                        {
                            var column = this.Columns[index];
                            if (column.IsHidden)
                            {
                                continue;
                            }
                            column.SetupFilter(column, index);
                        }

                        for (var index = 0; index < this.Columns.Count; index++)
                        {
                            var column = this.Columns[index];
                            if (column.IsHidden)
                            {
                                continue;
                            }
                            if (!column.HideFilter && column.DrawFilter(configuration, column, index))
                            {
                                refresh = true;
                            }
                        }
                    }

                    if (this.UseClipper)
                    {
                        ImGuiListClipperPtr clipper;
                        unsafe
                        {
                            clipper = new ImGuiListClipperPtr(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
                            clipper.ItemsHeight = 32;
                        }

                        _ = this.GetFilteredItemsAsync(configuration);
                        var filteredItems = this.items;
                        clipper.Begin(filteredItems.Count);
                        while (clipper.Step())
                        {
                            for (var index = clipper.DisplayStart; index < clipper.DisplayEnd; index++)
                            {
                                using var rowId = ImRaii.PushId(index);
                                if (index >= 0 && index < filteredItems.Count)
                                {
                                    var item = filteredItems[index];
                                    ImGui.TableNextRow(ImGuiTableRowFlags.None, 32);
                                    for (var columnIndex = 0; columnIndex < this.Columns.Count; columnIndex++)
                                    {
                                        using var colId = ImRaii.PushId(columnIndex);
                                        var column = this.Columns[columnIndex];
                                        if (column.IsHidden)
                                        {
                                            continue;
                                        }

                                        var columnMessages = column.Draw(
                                            configuration,
                                            item,
                                            index,
                                            columnIndex);
                                        if (this.HasFooter)
                                        {
                                            this.ColumnWidths[columnIndex] = ImGui.GetContentRegionAvail().X;
                                        }

                                        if (columnMessages != null)
                                        {
                                            messages.AddRange(columnMessages);
                                        }

                                        if (this.RightClickFunc != null && columnIndex == 0)
                                        {
                                            ImGui.SameLine();
                                            var hoveredRow = -1;
                                            var available = ImGui.GetFrameHeightWithSpacing();
                                            ImGui.Selectable(
                                                "",
                                                false,
                                                ImGuiSelectableFlags.SpanAllColumns |
                                                ImGuiSelectableFlags.AllowItemOverlap,
                                                new Vector2(0, available));
                                            if (ImGui.IsItemHovered(
                                                    ImGuiHoveredFlags.AllowWhenDisabled &
                                                    ImGuiHoveredFlags.AllowWhenOverlapped &
                                                    ImGuiHoveredFlags.AllowWhenBlockedByPopup &
                                                    ImGuiHoveredFlags.AllowWhenBlockedByActiveItem) &&
                                                ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                                            {
                                                ImGui.OpenPopup("RightClick" + index);
                                            }

                                            using (var popup = ImRaii.Popup("RightClick" + index))
                                            {
                                                if (popup.Success)
                                                {
                                                    messages.AddRange(this.RightClickFunc.Invoke(item));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        clipper.End();
                        clipper.Destroy();
                    }
                    else
                    {
                        _ = this.GetFilteredItemsAsync(configuration);
                        var filteredItems = this.items;
                        for (var index = 0; index < filteredItems.Count; index++)
                        {
                            using var rowId = ImRaii.PushId(index);
                            if (index >= 0 && index < filteredItems.Count)
                            {
                                var item = filteredItems[index];
                                ImGui.TableNextRow(ImGuiTableRowFlags.None, 32);
                                for (var columnIndex = 0; columnIndex < this.Columns.Count; columnIndex++)
                                {
                                    using var colId = ImRaii.PushId(columnIndex);
                                    var column = this.Columns[columnIndex];
                                    if (column.IsHidden)
                                    {
                                        continue;
                                    }

                                    var columnMessages = column.Draw(
                                        configuration,
                                        item,
                                        index,
                                        columnIndex);
                                    if (this.HasFooter)
                                    {
                                        this.ColumnWidths[columnIndex] = ImGui.GetContentRegionAvail().X;
                                    }

                                    if (columnMessages != null)
                                    {
                                        messages.AddRange(columnMessages);
                                    }

                                    if (this.RightClickFunc != null && columnIndex == 0)
                                    {
                                        ImGui.SameLine();
                                        var hoveredRow = -1;
                                        var available = ImGui.GetFrameHeightWithSpacing();
                                        ImGui.Selectable(
                                            "",
                                            false,
                                            ImGuiSelectableFlags.SpanAllColumns |
                                            ImGuiSelectableFlags.AllowItemOverlap,
                                            new Vector2(0, available));
                                        if (ImGui.IsItemHovered(
                                                ImGuiHoveredFlags.AllowWhenDisabled &
                                                ImGuiHoveredFlags.AllowWhenOverlapped &
                                                ImGuiHoveredFlags.AllowWhenBlockedByPopup &
                                                ImGuiHoveredFlags.AllowWhenBlockedByActiveItem) &&
                                            ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                                        {
                                            ImGui.OpenPopup("RightClick" + index);
                                        }

                                        using (var popup = ImRaii.Popup("RightClick" + index))
                                        {
                                            if (popup.Success)
                                            {
                                                messages.AddRange(this.RightClickFunc.Invoke(item));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (refresh)
                    {
                        this.IsDirty = true;
                    }
                }
            }
        }

        return messages;
    }

    public List<TMessageBase> DrawFooter(TConfiguration configuration, Vector2 size, bool shouldDraw = true)
    {
        var messages = new List<TMessageBase>();

        if (this.Columns.Count == 0 || !shouldDraw)
        {
            return messages;
        }

        using (var filterTableChild = ImRaii.Child(
                   "FilterTableFooter",
                   size * ImGui.GetIO().FontGlobalScale,
                   false,
                   ImGuiWindowFlags.NoScrollbar))
        {
            if (filterTableChild.Success)
            {
                using var table = ImRaii.Table(this.Key, this.Columns.Count, this.TableFlags & ImGuiTableFlags.ScrollX & ImGuiTableFlags.ScrollY);
                if (table.Success)
                {
                    var refresh = false;

                    for (var index = 0; index < this.Columns.Count; index++)
                    {
                        var column = this.Columns[index];
                        ImGui.TableSetupColumn(column.RenderName ?? this.Name, ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoResize, this.ColumnWidths.TryGetValue(index, out var value) ? value : column.Width, (uint)index);
                    }

                    var filteredItems = this.GetFilteredItems(configuration);
                    ImGui.TableNextRow(ImGuiTableRowFlags.None, 32);
                    for (var columnIndex = 0; columnIndex < this.Columns.Count; columnIndex++)
                    {
                        var column = this.Columns[columnIndex];
                        messages.AddRange(column.DrawFooter(configuration, filteredItems, columnIndex));
                    }
                }
            }
        }

        return messages;
    }

    public void Dispose()
    {
        this.SearchFilter.PropertyChanged -= this.SearchFilterModified;
    }
}