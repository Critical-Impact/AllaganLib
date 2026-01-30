using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.Grid;

public interface IRenderTable<TConfiguration, TMessageBase> where TConfiguration : INotifyPropertyChanged
{
    ImGuiTableFlags TableFlags { get; set; }
    
    string Name { get; set; }
    
    string Key { get; set; }
    
    int? FreezeCols { get; set; }
    
    int? FreezeRows { get; set; }
    
    int? SortColumn { get; set; }
    
    bool ShowFilterRow { get; set; }
    
    bool HasFooter { get; set; }
    
    bool IsLoading { get; }
    
    ImGuiSortDirection? SortDirection { get; set; }
    
    void SaveToCsv(TConfiguration configuration, RenderTableCsvExportOptions exportOptions);
    
    List<TMessageBase> Draw(TConfiguration configuration, Vector2 size, bool shouldDraw = true);
    
    List<TMessageBase> DrawFooter(TConfiguration configuration, Vector2 size, bool shouldDraw = true);
}

public interface IRenderTable<TConfiguration, TData, TMessageBase> : IRenderTable<TConfiguration, TMessageBase> where TConfiguration : INotifyPropertyChanged
{
    public List<IColumn<TConfiguration, TData, TMessageBase>> Columns { get; set; }
    
    public Func<TData, List<TMessageBase>>? RightClickFunc { get; set; }
    
    public List<TData> GetItems();
    public Task<List<TData>> GetItemsAsync();

    public List<TData> GetFilteredItems(TConfiguration configuration, bool isDirty = false);
}