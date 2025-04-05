using System;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace AllaganLib.Interface.Widgets;

public enum TimeUnit
{
    Seconds,
    Minutes,
    Hours,
    Days,
    Months,
    Years
}

public class TimeSpanPickerWidget
{
    private const float ColumnWidth = 150f;
    private readonly string emptyMessage;
    private readonly bool allowEmpty;
    private int tempTimeValue;
    private TimeUnit tempSelectedUnit;

    public TimeSpanPickerWidget(
        string emptyMessage = "No time selected",
        bool allowEmpty = true)
    {
        this.emptyMessage = emptyMessage;
        this.allowEmpty = allowEmpty;
    }

    public bool Draw(string id, ref TimeUnit? timeUnit, ref int? timeValue, int? inputSize = null)
    {
        var result = false;
        var formattedTimeSpan = timeUnit.HasValue && timeValue.HasValue
            ? $"Since {timeValue} {timeUnit.Value.ToString().ToLower()} ago"
            : this.emptyMessage;

        var buttonLabel = $"{formattedTimeSpan}";
        if (this.InputTextWithButton(buttonLabel, "...", inputSize))
        {
            this.tempTimeValue = timeValue ?? 1;
            this.tempSelectedUnit = timeUnit ?? TimeUnit.Seconds;

            ImGui.OpenPopup($"Time Span Picker##{id}");
        }

        ImGui.SameLine();
        var columnWidth = (ColumnWidth * 1.5f) + 20;
        if (this.allowEmpty)
        {
            columnWidth += ColumnWidth;
        }

        columnWidth *= ImGui.GetIO().FontGlobalScale;

        ImGui.SetNextWindowSizeConstraints(
            new Vector2(columnWidth, 0),
            new Vector2(columnWidth, 300));

        ImGui.SetNextWindowPos(ImGui.GetCursorScreenPos());

        if (ImGui.BeginPopup($"Time Span Picker##{id}"))
        {
            var tempSelectedUnitInt = (int)this.tempSelectedUnit;
            ImGui.Columns(2, null, true);
            ImGui.SetColumnWidth(0, ColumnWidth * 0.6f * ImGui.GetIO().FontGlobalScale);
            ImGui.SetColumnWidth(1, ColumnWidth * ImGui.GetIO().FontGlobalScale);

            ImGui.Text(this.tempSelectedUnit.ToString());
            ImGui.NextColumn();
            ImGui.PushItemWidth(ColumnWidth * 0.9f * ImGui.GetIO().FontGlobalScale);
            ImGui.InputInt("##TimeValue", ref this.tempTimeValue);
            ImGui.PopItemWidth();
            ImGui.NextColumn();

            ImGui.Text("Unit");
            ImGui.NextColumn();
            ImGui.PushItemWidth(ColumnWidth * 0.9f * ImGui.GetIO().FontGlobalScale);
            if (ImGui.Combo(
                    "##Unit",
                    ref tempSelectedUnitInt,
                    Enum.GetNames(typeof(TimeUnit)),
                    Enum.GetNames(typeof(TimeUnit)).Length))
            {
                this.tempSelectedUnit = (TimeUnit)tempSelectedUnitInt;
            }

            ImGui.PopItemWidth();

            ImGui.NextColumn();
            ImGui.NextColumn();
            ImGui.PushItemWidth(ColumnWidth * 0.9f * ImGui.GetIO().FontGlobalScale);
            if (ImGui.SmallButton($"Past 7 Days##{id}_Past7Days"))
            {
                this.tempTimeValue = 7;
                this.tempSelectedUnit = TimeUnit.Days;
            }

            ImGui.NextColumn();
            ImGui.NextColumn();
            ImGui.PushItemWidth(ColumnWidth * 0.9f * ImGui.GetIO().FontGlobalScale);
            if (ImGui.SmallButton($"Past 14 Days##{id}_Past14Days"))
            {
                this.tempTimeValue = 14;
                this.tempSelectedUnit = TimeUnit.Days;
            }

            ImGui.NextColumn();
            ImGui.NextColumn();
            ImGui.PushItemWidth(ColumnWidth * 0.9f * ImGui.GetIO().FontGlobalScale);
            if (ImGui.SmallButton($"Past Month##{id}_PastMonth"))
            {
                this.tempTimeValue = 1;
                this.tempSelectedUnit = TimeUnit.Months;
            }

            ImGui.Columns(1);
            ImGui.Dummy(new Vector2(0.0f, 8.0f * ImGui.GetIO().FontGlobalScale));

            if (ImGui.Button("OK", new Vector2(ColumnWidth * 0.75f * ImGui.GetIO().FontGlobalScale, 0)))
            {
                timeUnit = this.tempSelectedUnit;
                timeValue = this.tempTimeValue;
                ImGui.CloseCurrentPopup();
                result = true;
            }

            ImGui.SameLine();

            if (ImGui.Button("Cancel", new Vector2(ColumnWidth * 0.75f * ImGui.GetIO().FontGlobalScale, 0)))
            {
                ImGui.CloseCurrentPopup();
            }

            if (this.allowEmpty)
            {
                ImGui.SameLine();
                if (ImGui.Button("Reset", new Vector2(ColumnWidth * 0.75f * ImGui.GetIO().FontGlobalScale, 0)))
                {
                    timeUnit = null;
                    timeValue = null;
                    ImGui.CloseCurrentPopup();
                }
            }

            ImGui.EndPopup();
        }

        return result;
    }

    private bool InputTextWithButton(string label, string buttonLabel, int? inputSize = null)
    {
        var success = false;
        using var pushedId = ImRaii.PushId(label);
        using var pushedStyle = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        var cursorPosY = ImGui.GetCursorPosY();
        using (ImRaii.Disabled())
        {
            ImGui.SetNextItemWidth(inputSize ?? 100 * ImGui.GetIO().FontGlobalScale);
            ImGui.InputText($"##{label}_input", ref label, 100, ImGuiInputTextFlags.ReadOnly);
        }

        ImGui.SameLine();

        ImGui.SetCursorPosY(cursorPosY);

        if (ImGui.Button(buttonLabel))
        {
            success = true;
        }

        return success;
    }
}