using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

namespace AllaganLib.Shared.Windows;

public class AllaganDebugWindow : Window
{
    private readonly List<IDebugPane> debugWindows;
    private int selectedIndex;

    public AllaganDebugWindow(IEnumerable<IDebugPane> debugWindows)
        : base("Allagan Debug##AllaganLib.Shared")
    {
        this.debugWindows = debugWindows.OrderBy(c => c.Name).ToList();
        this.Size = new Vector2(800, 500);
        this.SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void Draw()
    {
        using (var sideBar = ImRaii.Child("##sidebbar", new Vector2(200, 0) * ImGui.GetIO().FontGlobalScale, true))
        {
            if (sideBar)
            {
                for (int i = 0; i < this.debugWindows.Count; i++)
                {
                    var isSelected = i == this.selectedIndex;
                    if (ImGui.Selectable(this.debugWindows[i].Name, isSelected))
                    {
                        this.selectedIndex = i;
                    }
                }
            }
        }

        ImGui.SameLine();
        using (var content = ImRaii.Child("##content", new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale))
        {
            if (content)
            {
                if (this.selectedIndex >= 0 && this.selectedIndex < this.debugWindows.Count)
                {
                    this.debugWindows[this.selectedIndex].Draw();
                }
                else
                {
                    ImGui.Text("No debug window selected.");
                }
            }
        }
    }
}