using System.Collections.Generic;
using System.Numerics;
using AllaganLib.Interface.FormFields;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace AllaganLib.Interface.Grid.ColumnFilters;

public class ChoiceColumnFilter
{
    public List<string> Choices { get; }

    public ChoiceColumnFilter(List<string> choices)
    {
        this.Choices = choices;
    }

    public void Setup<TConfiguration, TData, TMessageBase>(IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex)
    {
        ImGui.TableSetupColumn(column.Key + "Filter", ImGuiTableColumnFlags.NoSort);
    }

    public bool Draw<TConfiguration, TData, TMessageBase>(
        TConfiguration configuration,
        IColumn<TConfiguration, TData, TMessageBase> column,
        int columnIndex)
        where TConfiguration : IConfigurable<string?>
    {
        var hasChanged = false;
        ImGui.TableSetColumnIndex(columnIndex);
        ImGui.PushItemWidth(-20.000000f);
        using (ImRaii.PushId(column.Name))
        {
            using (ImRaii.PushStyle(ImGuiStyleVar.FramePadding, new Vector2(0, 0)))
            {
                var currentItem = configuration.Get(column.Key);

                using (var combo = ImRaii.Combo("##Choice", currentItem))
                {
                    if (combo.Success)
                    {
                        if (ImGui.Selectable("", false))
                        {
                            configuration.Set(column.Key, null);
                            hasChanged = true;
                        }

                        foreach (var choice in this.Choices)
                        {
                            if (ImGui.Selectable(choice, currentItem == choice))
                            {
                                configuration.Set(column.Key, choice);
                                hasChanged = true;
                            }
                        }
                    }
                }
            }

            ImGui.SameLine(0.0f, ImGui.GetStyle().ItemInnerSpacing.X);
            ImGui.TableHeader("");
        }

        ImGui.PopItemWidth();
        return hasChanged;
    }
}