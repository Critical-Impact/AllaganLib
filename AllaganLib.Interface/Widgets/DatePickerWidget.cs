using System;
using System.Numerics;

using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.Widgets;

public class DatePickerWidget
{
    private const float ColumnWidth = 100f;
    private readonly string dateFormat;
    private readonly string emptyMessage;
    private readonly bool allowEmpty;
    private int tempYear;
    private int tempMonth;
    private int tempDay;

    public DatePickerWidget(string dateFormat = "yyyy-MM-dd", string emptyMessage = "Not Set", bool allowEmpty = true)
    {
        this.dateFormat = dateFormat;
        this.emptyMessage = emptyMessage;
        this.allowEmpty = allowEmpty;
    }

    public bool Draw(string id, ref DateTime? newDate, int? inputSize = null)
    {
        var wasUpdated = false;
        var formattedDate = newDate?.ToString(this.dateFormat) ?? this.emptyMessage;
        var buttonLabel = $"{formattedDate}##{id}";
        var popupName = $"Date Picker##{id}";
        if (this.InputTextWithButton(buttonLabel, formattedDate, "...", inputSize))
        {
            if (newDate != null)
            {
                this.tempYear = newDate.Value.Year;
                this.tempMonth = newDate.Value.Month - 1;
                this.tempDay = newDate.Value.Day;
            }
            else
            {
                this.Today();
            }

            ImGui.OpenPopup(popupName);
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

        using (var popup = ImRaii.Popup(popupName))
        {
            if (popup.Success)
            {
                string[] months =
                {
                    "January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
                    "November", "December",
                };
                int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

                ImGui.Columns(2, border: true);
                ImGui.SetColumnWidth(0, ColumnWidth * 0.5f);
                ImGui.SetColumnWidth(1, ColumnWidth + 20);

                ImGui.Text("Year");
                ImGui.NextColumn();
                ImGui.PushItemWidth(ColumnWidth);
                ImGui.InputInt("##Year", ref this.tempYear);
                ImGui.PopItemWidth();
                ImGui.NextColumn();

                ImGui.Text("Month");
                ImGui.NextColumn();
                ImGui.PushItemWidth(ColumnWidth);
                ImGui.Combo("##Month", ref this.tempMonth, months, months.Length);
                ImGui.PopItemWidth();
                ImGui.NextColumn();

                ImGui.Text("Day");
                ImGui.NextColumn();
                ImGui.PushItemWidth(ColumnWidth);
                ImGui.InputInt("##Day", ref this.tempDay);
                ImGui.PopItemWidth();
                ImGui.NextColumn();

                var isLeapYear = (this.tempYear % 4 == 0 && this.tempYear % 100 != 0) || this.tempYear % 400 == 0;
                var maxDays = daysInMonth[this.tempMonth];
                if (this.tempMonth == 1 && isLeapYear)
                {
                    maxDays = 29;
                }

                if (this.tempDay < 1)
                {
                    this.tempDay = 1;
                }

                if (this.tempDay > maxDays)
                {
                    this.tempDay = maxDays;
                }

                if (this.tempYear > 9999)
                {
                    this.tempYear = 9999;
                }

                var selectedDate = new DateTime(this.tempYear, this.tempMonth + 1, this.tempDay);
                string[] daysOfWeek = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
                var dayOfWeek = daysOfWeek[(int)selectedDate.DayOfWeek];

                ImGui.NextColumn();
                ImGui.Text(dayOfWeek);

                ImGui.NextColumn();
                ImGui.NextColumn();
                if (ImGui.Button("Today", new Vector2(ColumnWidth * 0.75f, 0)))
                {
                    this.Today();
                }

                ImGui.Columns(1);

                ImGui.Dummy(new Vector2(0.0f, 8.0f));

                if (ImGui.Button("OK", new Vector2(ColumnWidth * 0.75f, 0)))
                {
                    newDate = new DateTime(this.tempYear, this.tempMonth + 1, this.tempDay);
                    ImGui.CloseCurrentPopup();
                    wasUpdated = true;
                }

                ImGui.SameLine();

                if (ImGui.Button("Cancel", new Vector2(ColumnWidth * 0.75f, 0)))
                {
                    ImGui.CloseCurrentPopup();
                }

                if (this.allowEmpty)
                {
                    ImGui.SameLine();

                    if (ImGui.Button("Reset", new Vector2(ColumnWidth * 0.75f, 0)))
                    {
                        ImGui.CloseCurrentPopup();
                        newDate = null;
                        wasUpdated = true;
                    }
                }
            }
        }

        return wasUpdated;
    }

    private void Today()
    {
        var now = DateTime.Now;
        this.tempYear = now.Year;
        this.tempMonth = now.Month - 1; // Adjust for 0-based month
        this.tempDay = now.Day;
    }

    private bool InputTextWithButton(string label, string? inputText, string buttonLabel, int? inputSize = null)
    {
        var success = false;
        inputText ??= this.emptyMessage;
        using var pushedId = ImRaii.PushId(label);
        using var pushedStyle = ImRaii.PushStyle(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        using (ImRaii.Disabled())
        {
            ImGui.SetNextItemWidth(inputSize ?? 200 * ImGui.GetIO().FontGlobalScale);
            ImGui.InputText(
                $"##{label}_input",
                ref inputText,
                100,
                ImGuiInputTextFlags.ReadOnly);
        }

        ImGui.SameLine();

        if (ImGui.Button(buttonLabel))
        {
            success = true;
        }

        return success;
    }
}