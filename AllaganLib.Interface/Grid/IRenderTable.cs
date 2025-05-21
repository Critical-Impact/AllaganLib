using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;

namespace AllaganLib.Interface.Grid;

public interface IRenderTable<TConfiguration, TData, TMessageBase> where TConfiguration : INotifyPropertyChanged
{
    public List<IColumn<TConfiguration, TData, TMessageBase>> Columns { get; set; }

    public ImGuiTableFlags TableFlags { get; set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public int? FreezeCols { get; set; }

    public int? FreezeRows { get; set; }

    public int? SortColumn { get; set; }

    public bool ShowFilterRow { get; set; }
    public bool HasFooter { get; set; }

    public bool IsLoading { get; }

    public Func<TData, List<TMessageBase>>? RightClickFunc { get; set; }

    public ImGuiSortDirection? SortDirection { get; set; }

    public List<TData> GetItems();
    public Task<List<TData>> GetItemsAsync();

    public List<TData> GetFilteredItems(TConfiguration configuration, bool isDirty = false);

    public void SaveToCsv(TConfiguration configuration, RenderTableCsvExportOptions exportOptions);

    public List<TMessageBase> Draw(TConfiguration configuration, Vector2 size, bool shouldDraw = true);
    public List<TMessageBase> DrawFooter(TConfiguration configuration, Vector2 size, bool shouldDraw = true);
}