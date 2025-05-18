using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Services;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using ImGuiNET;

namespace AllaganLib.Interface.Grid;

public abstract class IconColumn<TConfiguration, TData, TMessageBase> : IntegerFormField<TConfiguration>, IValueColumn<TConfiguration, TData, TMessageBase, int?>
    where TConfiguration : IConfigurable<int?>, INotifyPropertyChanged
{
    private readonly ITextureProvider textureProvider;

    protected IconColumn(ITextureProvider textureProvider, ImGuiService imGuiService)
        : base(imGuiService)
    {
        this.textureProvider = textureProvider;
        this.HideReset = true;
        this.ColourModified = false;
    }

    public abstract string? RenderName { get; set; }

    public abstract int Width { get; set; }

    public abstract bool HideFilter { get; set; }

    public virtual bool IsHidden { get; set; }

    public abstract ImGuiTableColumnFlags ColumnFlags { get; set; }

    public abstract string EmptyText { get; set; }

    public abstract Vector2 IconSize { get; set; }

    public virtual IEnumerable<TMessageBase>? Draw(TConfiguration config, TData item, int rowIndex, int columnIndex)
    {
        ImGui.TableNextColumn();
        if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
        {
            var currentValue = this.CurrentValue(item);
            if (currentValue != null)
            {
                bool isHq = currentValue > 500000;
                currentValue %= 500000;

                ImGui.AlignTextToFramePadding();
                ImGui.Image(this.textureProvider.GetFromGameIcon(new GameIconLookup((uint)currentValue, isHq)).GetWrapOrEmpty().ImGuiHandle, this.IconSize);
            }
            else
            {
                ImGui.TextUnformatted(this.EmptyText);
            }
        }

        return null;
    }

    public virtual List<TMessageBase> DrawFooter(TConfiguration config, List<TData> item, int columnIndex)
    {
        ImGui.TableNextColumn();
        return new List<TMessageBase>();
    }

    public bool DrawFilter(TConfiguration configuration, IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex)
    {
        return false;
    }

    public void SetupFilter(IColumn<TConfiguration, TData, TMessageBase> column, int columnIndex)
    {
    }

    public virtual IEnumerable<TData> Sort(
        TConfiguration configuration,
        IEnumerable<TData> items,
        ImGuiSortDirection direction)
    {
        return items;
    }

    public virtual IEnumerable<TData> Filter(TConfiguration config, IEnumerable<TData> items)
    {
        return items;
    }

    public virtual string CsvExport(TData item)
    {
        return string.Empty;
    }

    public abstract int? CurrentValue(TData item);
}
