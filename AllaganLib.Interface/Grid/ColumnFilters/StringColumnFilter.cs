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
        ImGui.PushItemWidth(-20.000000f);
        ImGui.PushID(column.Name);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
        ImGui.InputText("##" + column.Key + "FilterI" + column.Name, ref filter, this.maxFilterLength);
        ImGui.PopStyleVar();
        ImGui.SameLine(0.0f, ImGui.GetStyle().ItemInnerSpacing.X);
        ImGui.TableHeader(string.Empty);
        ImGui.PopID();
        ImGui.PopItemWidth();
        if (filter != (configuration.Get(column.Key) ?? string.Empty))
        {
            configuration.Set(column.Key, filter ?? null);
            hasChanged = true;
        }

        return hasChanged;
    }
}