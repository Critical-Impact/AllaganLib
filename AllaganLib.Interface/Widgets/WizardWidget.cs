using System.Collections.Generic;
using System.Numerics;

using AllaganLib.Interface.Services;
using AllaganLib.Interface.Wizard;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.Widgets;

/// <summary>
/// A widget that assists with configuring your plugin.
/// </summary>
/// <typeparam name="T">A configuration class that implements IWizardConfiguration.</typeparam>
public class WizardWidget<T>
    where T : class, IWizardConfiguration
{
    private readonly WizardWidgetSettings widgetSettings;
    private readonly ImGuiService imGuiService;
    private readonly IConfigurationWizardService<T> configurationWizardService;
    private readonly T configuration;
    private List<IFeature<T>> availableFeatures;
    private int currentFeature;

    /// <summary>
    /// Initializes a new instance of the <see cref="WizardWidget{T}"/> class.
    /// </summary>
    /// <param name="widgetSettings">The configuration of the widget.</param>
    /// <param name="imGuiService">An instance of ImGuiService.</param>
    /// <param name="configuration">A configuration class that implements IWizardConfiguration.</param>
    /// <param name="configurationWizardService">A configuration wizard that implements IConfigurationWizardService.</param>
    public WizardWidget(
        WizardWidgetSettings widgetSettings,
        ImGuiService imGuiService,
        T configuration,
        IConfigurationWizardService<T> configurationWizardService)
    {
        this.widgetSettings = widgetSettings;
        this.imGuiService = imGuiService;
        this.configurationWizardService = configurationWizardService;
        this.configuration = configuration;
        this.Initialize();
    }

    /// <summary>
    /// A delegate for when the widget closes.
    /// </summary>
    public delegate void ClosedDelegate();

    /// <summary>
    /// An event that gets called when the widget closes.
    /// </summary>
    public event ClosedDelegate? OnClosed;

    private bool CanGoPrevious => this.currentFeature != 0;

    private bool CanGoNext => this.availableFeatures.Count != 0 && this.currentFeature != this.availableFeatures.Count;

    public void Initialize()
    {
        this.availableFeatures = this.configurationWizardService.GetNewFeatures();
    }

    /// <summary>
    /// Draws the widget.
    /// </summary>
    public void Draw()
    {
        using (var sideBar = ImRaii.Child("sideBar", new Vector2(200, 0) * ImGui.GetIO().FontGlobalScale, true))
        {
            if (sideBar)
            {
                using (var sideBarMenu = ImRaii.Child(
                           "sideBarMenu",
                           new Vector2(200, -195) * ImGui.GetIO().FontGlobalScale,
                           false))
                {
                    if (sideBarMenu)
                    {
                        using (ImRaii.PushColor(
                                   ImGuiCol.Text,
                                   ImGuiColors.HealerGreen,
                                   this.currentFeature == 0))
                        {
                            ImGui.Text("Welcome");
                        }

                        for (var index = 0; index < this.availableFeatures.Count; index++)
                        {
                            var feature = this.availableFeatures[index];
                            using (ImRaii.PushColor(
                                       ImGuiCol.Text,
                                       ImGuiColors.HealerGreen,
                                       index + 1 == this.currentFeature))
                            {
                                ImGui.Text(index + 1 + ". " + feature.Name);
                            }
                        }
                    }
                }

                using (var sideBarImage = ImRaii.Child(
                           "sideBarImage",
                           new Vector2(200, 0) * ImGui.GetIO().FontGlobalScale,
                           false,
                           ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
                {
                    if (sideBarImage)
                    {
                        ImGui.Separator();
                        ImGui.Image(
                            this.imGuiService.LoadImage(this.widgetSettings.LogoPath).GetWrapOrEmpty().Handle,
                            new Vector2(190, 190));
                    }
                }
            }
        }

        ImGui.SameLine();
        using (var mainWindow = ImRaii.Child("mainWindow", new Vector2(0, 0)))
        {
            if (mainWindow)
            {
                using (var mainContainer = ImRaii.Child(
                           "mainContainer",
                           new Vector2(-1, -80) * ImGui.GetIO().FontGlobalScale,
                           true))
                {
                    if (mainContainer)
                    {
                        if (this.currentFeature == 0)
                        {
                            if (this.configurationWizardService.ConfiguredOnce)
                            {
                                ImGui.TextWrapped(
                                    $"Welcome back to the {this.widgetSettings.PluginName} configuration wizard.");
                                ImGui.Separator();
                                ImGui.TextWrapped(
                                    "There are new features available to configure and you elected to show this window when that occurs.");
                                ImGui.NewLine();
                            }
                            else
                            {
                                ImGui.TextWrapped(
                                    $"Welcome to the {this.widgetSettings.PluginName} configuration wizard.");
                                ImGui.Separator();
                                ImGui.TextWrapped(
                                    "This will guide you through the setup of the most commonly used features. This wizard, with your permission will show itself again when a new feature gets released as features are normally left for the user to configure and activate.");
                            }
                        }
                        else
                        {
                            for (var index = 0; index < this.availableFeatures.Count; index++)
                            {
                                var feature = this.availableFeatures[index];
                                if (this.currentFeature - 1 == index)
                                {
                                    ImGui.Text(feature.Name);
                                    ImGui.Separator();
                                    ImGui.PushTextWrapPos();
                                    ImGui.Text(feature.Description);
                                    ImGui.PopTextWrapPos();
                                    ImGui.Separator();
                                    foreach (var setting in this.configurationWizardService.GetApplicableSettings(
                                                 feature))
                                    {
                                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                        setting.LabelSize = (int)(ImGui.GetWindowContentRegionMax().X - 20);
                                        setting.Draw(this.configuration);
                                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                    }
                                }
                            }
                        }
                    }
                }

                using (var nextPrevBar = ImRaii.Child(
                           "nextPrevBar",
                           new Vector2(-1, -1) * ImGui.GetIO().FontGlobalScale,
                           true))
                {
                    if (nextPrevBar)
                    {
                        if (this.currentFeature == 0)
                        {
                            if (this.configurationWizardService.ConfiguredOnce)
                            {
                                if (ImGui.Button("Continue"))
                                {
                                    this.NextStep();
                                    this.configuration.ShowWizardNewFeatures = true;
                                }

                                if (ImGui.Button("Close (and show next time the plugin loads)"))
                                {
                                    this.OnClosed?.Invoke();
                                    this.configuration.ShowWizardNewFeatures = true;
                                }
                            }
                            else
                            {
                                if (ImGui.Button("Continue (and show on new features)"))
                                {
                                    this.NextStep();
                                    this.configuration.ShowWizardNewFeatures = true;
                                }

                                ImGui.SameLine();
                                if (ImGui.Button("Continue (and never show the wizard again)"))
                                {
                                    this.NextStep();
                                    this.configuration.ShowWizardNewFeatures = false;
                                }

                                if (ImGui.Button("Close (and show next time the plugin loads)"))
                                {
                                    this.OnClosed?.Invoke();
                                    this.configuration.ShowWizardNewFeatures = true;
                                }

                                ImGui.SameLine();
                                if (ImGui.Button("Close (and never show the wizard again)"))
                                {
                                    this.OnClosed?.Invoke();
                                    this.configuration.ShowWizardNewFeatures = false;
                                }
                            }
                        }
                        else
                        {
                            var canGoPrevious = this.CanGoPrevious;
                            if (!canGoPrevious)
                            {
                                ImGui.BeginDisabled();
                            }

                            if (ImGui.Button("Previous"))
                            {
                                this.PreviousStep();
                            }

                            if (!canGoPrevious)
                            {
                                ImGui.EndDisabled();
                            }

                            ImGui.SameLine();
                            var canGoNext = this.CanGoNext;

                            if (canGoNext && ImGui.Button("Next"))
                            {
                                this.NextStep();
                            }

                            if (!canGoNext && ImGui.Button("Finish"))
                            {
                                this.Finish();
                            }
                        }
                    }
                }
            }
        }
    }

    private void NextStep()
    {
        if (this.currentFeature == 0)
        {
            this.currentFeature = 1;
        }
        else if (this.currentFeature == this.availableFeatures.Count)
        {
        }
        else
        {
            this.currentFeature++;
        }
    }

    private void PreviousStep()
    {
        if (this.currentFeature != 0)
        {
            this.currentFeature--;
        }
    }

    private void Finish()
    {
        this.OnClosed?.Invoke();
        this.currentFeature = 0;
        foreach (var feature in this.availableFeatures)
        {
            feature.OnFinish();
        }

        this.configurationWizardService.MarkFeaturesSeen();
    }
}