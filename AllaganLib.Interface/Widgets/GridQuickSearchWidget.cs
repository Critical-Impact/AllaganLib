using System.ComponentModel;
using System.Linq;
using AllaganLib.Interface.Grid;
using ImGuiNET;

namespace AllaganLib.Interface.Widgets;

public class GridQuickSearchWidget<TConfiguration, TData, TMessageBase>
    where TConfiguration : INotifyPropertyChanged
{
    private readonly IRenderTable<TConfiguration, TData, TMessageBase> renderTable;

    public GridQuickSearchWidget(IRenderTable<TConfiguration, TData, TMessageBase> renderTable)
    {
        this.renderTable = renderTable;
    }

    public void Draw(TConfiguration configuration, int? labelSize = null, int? inputSize = null)
    {
        var columns = this.renderTable.Columns.Where(c => c.HideFilter != true).ToList();
        for (var index = 0; index < columns.Count; index++)
        {
            var column = columns[index];
            column.Draw(configuration, labelSize, inputSize);
            if (index != columns.Count - 1)
            {
                ImGui.Separator();
            }
        }
    }
}