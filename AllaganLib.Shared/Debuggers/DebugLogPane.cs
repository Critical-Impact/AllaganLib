using System;
using System.Collections.Generic;
using System.Numerics;
using AllaganLib.Shared.Debuggers;
using AllaganLib.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;

public abstract class DebugLogPane : SubscriberDebugPane
{
    private readonly List<string> eventLog = new();
    private readonly object logLock = new();

    /// <summary>
    /// Adds a line to the log with a timestamp.
    /// </summary>
    public void AddLog(string message)
    {
        lock (this.logLock)
        {
            this.eventLog.Add($"{DateTime.Now:HH:mm:ss} {message}");
            if (this.eventLog.Count > 200)
            {
                this.eventLog.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// The information to draw for this debug pane
    /// </summary>
    public abstract void DrawInfo();


    /// <inheritdoc/>
    public override void Draw()
    {
        var fullHeight = ImGui.GetContentRegionAvail().Y;
        var infoHeight = fullHeight * 0.60f;

        using (var info = ImRaii.Child("##info", new Vector2(0, infoHeight) * ImGui.GetIO().FontGlobalScale, true))
        {
            if (info)
            {
                this.DrawInfo();
            }
        }

        ImGui.Separator();

        using (var eventLog = ImRaii.Child(
                   "##eventlog",
                   new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale,
                   true))
        {
            if (eventLog)
            {
                using (var buttonBar = ImRaii.Child(
                           "##buttonbar",
                           new Vector2(0, 20) * ImGui.GetIO().FontGlobalScale,
                           false,
                           ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
                {
                    if (buttonBar)
                    {
                        if (ImGui.Button("Clear Log"))
                        {
                            lock (this.logLock)
                            {
                                this.eventLog.Clear();
                            }
                        }

                        ImGui.SameLine();
                        if (ImGui.Button("Subscribe to Events"))
                        {
                            lock (this.logLock)
                            {
                                this.SubscribeToEvents();
                            }
                        }

                        ImGui.SameLine();
                        if (ImGui.Button("Unsubscribe to Events"))
                        {
                            lock (this.logLock)
                            {
                                this.UnsubscribeAll();
                            }
                        }
                    }
                }

                ImGui.Separator();
                using (var eventlogbody = ImRaii.Child(
                           "##eventlogbody",
                           new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale))
                {
                    if (eventlogbody)
                    {
                        lock (this.logLock)
                        {
                            foreach (var entry in this.eventLog)
                            {
                                ImGui.TextUnformatted(entry);
                            }
                        }

                        if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                        {
                            ImGui.SetScrollHereY(1.0f);
                        }
                    }
                }
            }
        }
    }
}