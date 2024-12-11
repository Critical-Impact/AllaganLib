using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using AllaganLib.Interface.Services;
using AllaganLib.Shared.Comparers;
using AllaganLib.Shared.Extensions;
using Dalamud.Interface.Colors;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace AllaganLib.Interface.FormFields;

public abstract class GameColorFormField<T> : FormField<uint, T>
    where T : IConfigurable<uint?>
{
    private readonly Dictionary<uint, UIColor> uiColors;

    public override uint CurrentValue(T configurable)
    {
        return configurable.Get(this.Key) ?? this.DefaultValue;
    }

    public override void UpdateFilterConfiguration(T configurable, uint newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public GameColorFormField(ImGuiService imGuiService, IDataManager dataManager)
        : base(imGuiService)
    {
        var list = new List<UIColor>(dataManager.GetExcelSheet<UIColor>()!.Distinct(new UIColorComparer()));
        list.Sort(
            (a, b) =>
            {
                var colorA = a.ConvertUiColorToColor();
                var colorB = b.ConvertUiColorToColor();
                ImGui.ColorConvertRGBtoHSV(colorA.X, colorA.Y, colorA.Z, out var aH, out var aS, out var aV);
                ImGui.ColorConvertRGBtoHSV(colorB.X, colorB.Y, colorB.Z, out var bH, out var bS, out var bV);

                var hue = aH.CompareTo(bH);
                if (hue != 0)
                {
                    return hue;
                }

                var saturation = aS.CompareTo(bS);
                if (saturation != 0)
                {
                    return saturation;
                }

                var value = aV.CompareTo(bV);
                return value != 0 ? value : 0;
            });
        this.uiColors = list.ToDictionary(c => c.RowId, c => c);
    }

    public override bool DrawInput(T configuration, int? inputSize = null)
    {
        var value = this.CurrentValue(configuration);
        var currentColour = new Vector4(255, 255, 255, 255);
        var wasUpdated = false;

        if (this.uiColors.TryGetValue(value, out var toConvert))
        {
            currentColour = toConvert.ConvertUiColorToColor();
        }

        if (ImGui.ColorButton("##" + this.Key + "CurrentVal", currentColour))
        {
        }

        var index = 0;
        foreach (var uiColor in this.uiColors)
        {
            var z = uiColor.Value;
            if (z.UIForeground is 0 or 255)
            {
                continue;
            }

            var color = z.ConvertUiColorToColor();
            var id = z.RowId.ToString();
            var imGuiColorEditFlags = ImGuiColorEditFlags.NoBorder;
            if (value == z.RowId)
            {
                imGuiColorEditFlags = ImGuiColorEditFlags.None;
            }

            if (ImGui.ColorButton(id, color, imGuiColorEditFlags) && uint.TryParse(id, out var convertedId))
            {
                if (this.AutoSave)
                {
                    this.UpdateFilterConfiguration(configuration, convertedId);
                }

                wasUpdated = true;
            }

            index++;
            if (index % 10 != 0)
            {
                ImGui.SameLine();
            }
        }

        return wasUpdated;
    }
}