using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AllaganLib.Interface.Extensions;
using AllaganLib.Interface.Interfaces;
using AllaganLib.Interface.Services;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.FormFields;

public abstract class HotkeyFormField<T> : FormField<VirtualKey[], T>, IHotkey<T>
    where T : IConfigurable<VirtualKey[]?>
{
    private readonly IKeyState keyState;
    private readonly List<VirtualKey> newKeys = [];
    private readonly Stopwatch safety = Stopwatch.StartNew();
    private string? settingKey;
    private string? focused;
    private bool isEditing;
    private bool isEnabled = true;

    protected HotkeyFormField(ImGuiService imGuiService, IKeyState keyState)
        : base(imGuiService)
    {
        this.keyState = keyState;
    }

    public override VirtualKey[] CurrentValue(T configurable)
    {
        return configurable.Get(this.Key) ?? this.DefaultValue;
    }

    bool IHotkey<T>.IsEditing => this.isEditing;

    bool IHotkey<T>.IsEnabled => this.isEnabled;

    public override void UpdateFilterConfiguration(T configurable, VirtualKey[]? newValue)
    {
        configurable.Set(this.Key, newValue);
    }

    public override bool DrawInput(T configuration, int? inputSize = null)
    {
        var currentValue = this.CurrentValue(configuration);
        var wasUpdated = false;

        var hotkey = "##" + this.Key + "HotKey";
        var identifier = hotkey;
        var strKeybind = string.Join("+", currentValue.Select(k => k.GetKeyName()));


        if (this.settingKey == identifier)
        {
            if (this.keyState[VirtualKey.MENU] && !this.newKeys.Contains(VirtualKey.MENU))
            {
                this.newKeys.Add(VirtualKey.MENU);
            }

            if (this.keyState[VirtualKey.SHIFT] && !this.newKeys.Contains(VirtualKey.SHIFT))
            {
                this.newKeys.Add(VirtualKey.SHIFT);
            }

            if (this.keyState[VirtualKey.CONTROL] && !this.newKeys.Contains(VirtualKey.CONTROL))
            {
                this.newKeys.Add(VirtualKey.CONTROL);
            }

            foreach(var key in Enum.GetValues<ImGuiKey>())
            {
                var virtualKey = key.ToVirtualKey();
                if (virtualKey != VirtualKey.NO_KEY && this.keyState.IsVirtualKeyValid(virtualKey) && ImGui.IsKeyDown(key))
                {
                    if (!this.newKeys.Contains(virtualKey))
                    {
                        if (virtualKey == VirtualKey.ESCAPE)
                        {
                            this.settingKey = null;
                            this.newKeys.Clear();
                            this.focused = null;
                            break;
                        }

                        this.newKeys.Add(virtualKey);
                    }
                }
            }

            this.newKeys.Sort();
            strKeybind = string.Join("+", this.newKeys.Select(k => k.GetKeyName()));
        }
        var keybindText = "Set Keybind";
        var textSize = ImGui.CalcTextSize(keybindText);

        ImGui.SetNextItemWidth((this.InputSize - textSize.X) * ImGuiHelpers.GlobalScale);
        ImGui.InputText(hotkey, ref strKeybind, 100, ImGuiInputTextFlags.ReadOnly);

        var active = ImGui.IsItemActive();

        if (this.settingKey == identifier)
        {
            this.isEditing = true;
            if (this.focused != identifier)
            {
                ImGui.SetKeyboardFocusHere(-1);
                this.focused = identifier;
            }
            else
            {
                this.safety.Restart();
                ImGui.SameLine();

                if (ImGui.Button(this.newKeys.Count > 0 ? "Confirm" + $"##{identifier}" : "Cancel" + $"##{identifier}"))
                {
                    this.safety.Reset();
                    this.settingKey = null;
                    if (this.newKeys.Count > 0)
                    {
                        this.UpdateFilterConfiguration(configuration, this.newKeys.ToArray());
                        wasUpdated = true;
                    }

                    this.newKeys.Clear();
                }
                else
                {
                    if (!active)
                    {
                        this.safety.Reset();
                        this.focused = null;
                        this.settingKey = null;
                        if (this.newKeys.Count > 0)
                        {
                            this.UpdateFilterConfiguration(configuration, this.newKeys.ToArray());
                            wasUpdated = true;
                        }

                        this.newKeys.Clear();
                    }
                }
            }
        }
        else
        {
            this.isEditing = false;
            ImGui.SameLine();
            if (ImGui.Button(keybindText + $"###setHotkeyButton{identifier}"))
            {
                this.safety.Restart();
                this.settingKey = identifier;
            }
        }

        return wasUpdated;
    }
}