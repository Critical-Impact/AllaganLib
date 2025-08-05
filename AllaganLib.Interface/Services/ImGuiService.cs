using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

using Dalamud.Interface.Textures;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;

namespace AllaganLib.Interface.Services;

public class ImGuiService
{
    private readonly IDalamudPluginInterface pluginInterface;
    private readonly ITextureProvider textureProvider;

    public ImGuiService(IDalamudPluginInterface pluginInterface, ITextureProvider textureProvider)
    {
        this.pluginInterface = pluginInterface;
        this.textureProvider = textureProvider;
    }

    public ISharedImmediateTexture LoadImage(string imageName)
    {
        var assemblyLocation = this.pluginInterface.AssemblyLocation.DirectoryName!;
        var imagePath = Path.Combine(assemblyLocation, Path.Combine("Images", $"{imageName}.png"));
        return this.textureProvider.GetFromFile(new FileInfo(imagePath));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void HelpMarker(string helpText, string? imagePath = null, System.Numerics.Vector2? imageSize = null)
    {
        ImGui.TextDisabled("(?)");
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
            ImGui.TextUnformatted(helpText);
            ImGui.PopTextWrapPos();
            if (imagePath != null)
            {
                var sourceIcon = this.LoadImage(imagePath);
                ImGui.Image(
                    sourceIcon.GetWrapOrEmpty().Handle,
                    imageSize ??
                    new Vector2(200, 200) * ImGui.GetIO().FontGlobalScale);
            }

            ImGui.EndTooltip();
        }
    }
}