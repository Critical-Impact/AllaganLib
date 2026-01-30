using System.ComponentModel;
using System.Numerics;
using AllaganLib.Interface.FormFields;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.Grid.ColumnFilters;

public class StringColumnFilter
{
    private readonly int maxFilterLength;

    public StringColumnFilter(int maxFilterLength = 1000)
    {
        this.maxFilterLength = maxFilterLength;
    }
    public void Setup<TConfiguration, TData, TMessageBase>(IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex)
        where TConfiguration : INotifyPropertyChanged
    {
    }

    public bool Draw<TConfiguration, TData, TMessageBase>(
        TConfiguration configuration,
        IColumn<TConfiguration, TData, TMessageBase> column,
        int columnIndex)
        where TConfiguration : IConfigurable<string?>, INotifyPropertyChanged
    {
        var filter = configuration.Get(column.Key) ?? string.Empty;
        var hasChanged = false;

        ImGui.TableSetColumnIndex(columnIndex);
        ImGui.PushID(column.Name);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(2, 2));
        ImGui.SetNextItemWidth(-float.Epsilon);
        ImGui.InputText(
            "##Filter",
            ref filter,
            this.maxFilterLength
        );
        ImGui.PopStyleVar();
        ImGui.PopID();
        if (filter != (configuration.Get(column.Key) ?? string.Empty))
        {
            configuration.Set(column.Key, filter ?? null);
            hasChanged = true;
        }

        return hasChanged;
    }
}