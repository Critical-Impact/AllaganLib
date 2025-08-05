using System;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.Widgets;

public class VerticalSplitter
{
    private float _width;
    private float? _splitterResizeBuffer;
    private readonly Vector2 _maxRange;

    public float Width
    {
        get => this._width;
        set => this._width = Math.Clamp(value, this._maxRange.X, this._maxRange.Y);
    }

    public VerticalSplitter(float width, Vector2 maxRange)
    {
        this._maxRange = maxRange;
        this._width = width;
    }

    public float DraggerSize { get; set; } = 2;

    public void Draw(Action drawLeft, Action drawRight)
    {
        if (!ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            this._splitterResizeBuffer = null;
        }

        using (var leftChild = ImRaii.Child(
                   "Left",
                   new Vector2(this.Width, -1.0f) * ImGui.GetIO().FontGlobalScale,
                   true))
        {
            if (leftChild.Success)
            {
                drawLeft.Invoke();
            }
        }

        ImGui.SameLine();
        using (var dragger = ImRaii.Child("Dragger", new System.Numerics.Vector2(this.DraggerSize, 0), false))
        {
            if (dragger.Success)
            {
                ImGui.Button("DraggerBtn", new Vector2(-1, -1));
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEw);
                }
                if (ImGui.IsItemActive())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEw);

                    if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                    {
                        if (this._splitterResizeBuffer == null)
                        {
                            this._splitterResizeBuffer = this.Width;
                        }

                        var mouseDragDelta = ImGui.GetMouseDragDelta(ImGuiMouseButton.Left, 0);
                        this.Width = this._splitterResizeBuffer.Value + mouseDragDelta.X;
                    }
                }
            }
        }

        ImGui.SameLine();
        using (var rightChild = ImRaii.Child("Right", new Vector2(-1, -1) * ImGui.GetIO().FontGlobalScale, true))
        {
            if (rightChild.Success)
            {
                drawRight.Invoke();
            }
        }
    }
}