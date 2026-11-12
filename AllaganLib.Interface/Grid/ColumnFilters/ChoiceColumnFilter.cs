using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using AllaganLib.Interface.FormFields;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.Grid.ColumnFilters;

public class ChoiceColumnFilter
{
    private readonly Dictionary<string, string> labels;

    public ChoiceColumnFilter(List<string> choices)
    {
        this.Choices = choices;
        this.labels = new Dictionary<string, string>();
    }

    public ChoiceColumnFilter(List<(string Value, string Label)> choices)
    {
        this.Choices = choices.Select(c => c.Value).ToList();
        this.labels = choices.ToDictionary(c => c.Value, c => c.Label);
    }

    public List<string> Choices { get; }

    public string GetLabel(string choice)
    {
        return this.labels.GetValueOrDefault(choice, choice);
    }

    public void Setup<TConfiguration, TData, TMessageBase>(
        IColumn<TConfiguration, TData, TMessageBase> column,
        int columnIndex)
        where TConfiguration : INotifyPropertyChanged
    {
    }

    public bool Draw<TConfiguration, TData, TMessageBase>(
        TConfiguration configuration,
        IColumn<TConfiguration, TData, TMessageBase> column,
        int columnIndex)
        where TConfiguration : IConfigurable<string?>, INotifyPropertyChanged
    {
        var hasChanged = false;
        ImGui.TableSetColumnIndex(columnIndex);
        using (ImRaii.PushId(column.Name))
        {
            using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(2, 2)))
            {
                var currentItem = configuration.Get(column.Key) ?? "";

                ImGui.PushItemWidth(-float.Epsilon);
                using (var combo = ImRaii.Combo("##Choice", this.GetLabel(currentItem)))
                {
                    if (combo.Success)
                    {
                        if (ImGui.Selectable("##ClearChoice", false))
                        {
                            configuration.Set(column.Key, null);
                            hasChanged = true;
                        }

                        foreach (var choice in this.Choices)
                        {
                            if (ImGui.Selectable(this.GetLabel(choice), currentItem == choice))
                            {
                                configuration.Set(column.Key, choice);
                                hasChanged = true;
                            }
                        }
                    }
                }
                ImGui.PopItemWidth();
            }
        }

        return hasChanged;
    }
}
