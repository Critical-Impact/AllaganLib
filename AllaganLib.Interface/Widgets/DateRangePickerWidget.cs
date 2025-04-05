using System;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace AllaganLib.Interface.Widgets;

public class DateRangePickerWidget
{
    private const float ColumnWidth = 100f;
    private readonly string dateFormat;
    private readonly string emptyStartMessage;
    private readonly string emptyEndMessage;
    private readonly bool allowEmpty;

    private int tempStartYear;
    private int tempStartMonth;
    private int tempStartDay;
    private int tempEndYear;
    private int tempEndMonth;
    private int tempEndDay;

    public DateRangePickerWidget(
        string dateFormat = "yyyy-MM-dd",
        string emptyStartMessage = "Not Set",
        string emptyEndMessage = "Not Set",
        bool allowEmpty = true)
    {
        this.dateFormat = dateFormat;
        this.emptyStartMessage = emptyStartMessage;
        this.emptyEndMessage = emptyEndMessage;
        this.allowEmpty = allowEmpty;
    }

    public bool Draw(string id, ref DateTime? startDate, ref DateTime? endDate, int? inputSize = null)
    {
        var wasUpdated = false;
        var formattedStartDate =
            startDate.HasValue ? startDate.Value.ToString(this.dateFormat) : this.emptyStartMessage;
        var formattedEndDate = endDate.HasValue ? endDate.Value.ToString(this.dateFormat) : this.emptyEndMessage;

        var buttonLabel = $"{formattedStartDate} - {formattedEndDate}";
        if (this.InputTextWithButton(buttonLabel, "..."))
        {
            if (startDate.HasValue)
            {
                this.tempStartYear = startDate.Value.Year;
                this.tempStartMonth = startDate.Value.Month - 1;
                this.tempStartDay = startDate.Value.Day;
            }
            else
            {
                DateTime date = DateTime.Now;
                this.tempStartYear = date.Year;
                this.tempStartMonth = date.Month - 1;
                this.tempStartDay = date.Day;
            }

            if (endDate.HasValue)
            {
                this.tempEndYear = endDate.Value.Year;
                this.tempEndMonth = endDate.Value.Month - 1;
                this.tempEndDay = endDate.Value.Day;
            }
            else
            {
                DateTime date = DateTime.Now;
                this.tempEndYear = date.Year;
                this.tempEndMonth = date.Month - 1;
                this.tempEndDay = date.Day;
            }

            ImGui.OpenPopup(id);
        }

        ImGui.SameLine();
        ImGui.SetNextWindowSizeConstraints(
            new Vector2((ColumnWidth * 3f) + 40, 0) * ImGui.GetIO().FontGlobalScale,
            new Vector2((ColumnWidth * 3f) + 40, 400) * ImGui.GetIO().FontGlobalScale);
        ImGui.SetNextWindowPos(ImGui.GetCursorScreenPos());

        if (ImGui.BeginPopup(id))
        {
            ImGui.Columns(2, null, false);
            ImGui.SetColumnWidth(0, ((ColumnWidth * 0.5f) + ColumnWidth + 20) * ImGui.GetIO().FontGlobalScale);
            ImGui.SetColumnWidth(1, ((ColumnWidth * 0.5f) + ColumnWidth + 20) * ImGui.GetIO().FontGlobalScale);
            ImGui.NextColumn();
            this.DrawDateSelector("End Date", ref this.tempEndYear, ref this.tempEndMonth, ref this.tempEndDay);
            ImGui.SameLine();
            ImGui.NextColumn();
            this.DrawDateSelector("Start Date", ref this.tempStartYear, ref this.tempStartMonth, ref this.tempStartDay);
            ImGui.Columns(1);
            if (!this.IsValidDateRange())
            {
                ImGui.TextColored(new Vector4(1.0f, 0.0f, 0.0f, 1.0f), "Invalid date range.");
            }

            ImGui.Dummy(new Vector2(0.0f, 8.0f) * ImGui.GetIO().FontGlobalScale);

            if (ImGui.Button("OK", new Vector2(ColumnWidth * 0.75f, 0) * ImGui.GetIO().FontGlobalScale))
            {
                if (this.IsValidDateRange())
                {
                    startDate = new DateTime(this.tempStartYear, this.tempStartMonth + 1, this.tempStartDay);
                    endDate = new DateTime(this.tempEndYear, this.tempEndMonth + 1, this.tempEndDay);
                    ImGui.CloseCurrentPopup();
                    wasUpdated = true;
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("Cancel", new Vector2(ColumnWidth * 0.75f, 0) * ImGui.GetIO().FontGlobalScale))
            {
                ImGui.CloseCurrentPopup();
            }

            if (this.allowEmpty)
            {
                ImGui.SameLine();
                if (ImGui.Button("Reset", new Vector2(ColumnWidth * 0.75f, 0) * ImGui.GetIO().FontGlobalScale))
                {
                    startDate = null;
                    endDate = null;
                    ImGui.CloseCurrentPopup();
                    wasUpdated = true;
                }
            }

            ImGui.EndPopup();
        }

        return wasUpdated;
    }

    private void DrawDateSelector(string label, ref int year, ref int month, ref int day)
    {
        using (var child = ImRaii.Child(
                   label,
                   new Vector2(ImGui.GetWindowWidth() / 2, 120) * ImGui.GetIO().FontGlobalScale,
                   false,
                   ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            if (child)
            {
                string[] months =
                {
                    "January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
                    "November", "December"
                };
                int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

                ImGui.Text(label);
                ImGui.Columns(2, null, false);
                ImGui.SetColumnWidth(0, ColumnWidth * 0.5f);
                ImGui.SetColumnWidth(1, ColumnWidth + 20);

                ImGui.Text("Day");
                ImGui.NextColumn();
                ImGui.PushItemWidth(ColumnWidth * 0.9f);
                ImGui.InputInt($"##{label}_Day", ref day);
                ImGui.PopItemWidth();
                ImGui.NextColumn();

                ImGui.Text("Month");
                ImGui.NextColumn();
                ImGui.PushItemWidth(ColumnWidth * 0.9f);
                ImGui.Combo($"##{label}_Month", ref month, months, months.Length);
                ImGui.PopItemWidth();
                ImGui.NextColumn();

                ImGui.Text("Year");
                ImGui.NextColumn();
                ImGui.PushItemWidth(ColumnWidth * 0.9f);
                ImGui.InputInt($"##{label}_Year", ref year);
                ImGui.PopItemWidth();
                ImGui.NextColumn();

                ImGui.NextColumn();
                ImGui.PushItemWidth(ColumnWidth * 0.9f);
                if (ImGui.SmallButton($"Today##{label}_Today"))
                {
                    var date = DateTime.Now;
                    year = date.Year;
                    month = date.Month - 1;
                    day = date.Day;
                }

                ImGui.PopItemWidth();
                ImGui.NextColumn();

                var isLeapYear = (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;
                var maxDays = daysInMonth[month];
                if (month == 1 && isLeapYear)
                {
                    maxDays = 29;
                }

                if (day < 1)
                {
                    day = 1;
                }

                if (day > maxDays)
                {
                    day = maxDays;
                }

                ImGui.Columns(1);
            }
        }
    }

    private bool IsValidDateRange()
    {
        var startDate = new DateTime(this.tempStartYear, this.tempStartMonth + 1, this.tempStartDay);
        var endDate = new DateTime(this.tempEndYear, this.tempEndMonth + 1, this.tempEndDay);
        return startDate <= endDate;
    }

    private bool InputTextWithButton(string label, string buttonLabel, int? inputSize = null)
    {
        var success = false;
        using var pushedId = ImRaii.PushId(label);
        using var pushedStyle = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        var cursorPosY = ImGui.GetCursorPosY();
        using (ImRaii.Disabled())
        {
            ImGui.SetNextItemWidth(inputSize ?? 200 * ImGui.GetIO().FontGlobalScale);
            ImGui.InputText(
                $"##{label}_input",
                ref label,
                200,
                ImGuiInputTextFlags.ReadOnly);
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