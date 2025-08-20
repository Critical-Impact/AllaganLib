using System.Numerics;

namespace AllaganLib.Interface.FormFields;

public interface IFormField<TConfiguration>
{
    public int LabelSize { get; set; }

    public string Key { get; set; }

    public string Name { get; set; }

    public string HelpText { get; set; }

    public string WizardName { get; }

    public bool HideReset { get; set; }

    public bool ColourModified { get; set; }

    public string? Image { get; }

    public Vector2? ImageSize { get; }

    public string Version { get; }

    public bool HasValueSet(TConfiguration configuration);

    public bool AutoSave { get; }

    public bool Draw(TConfiguration configuration, int? labelSize = null, int? inputSize = null);

    public bool DrawInput(TConfiguration configuration, int? inputSize = null);

    public void DrawLabel(TConfiguration configuration, int? labelSize = null);

    public void DrawHelp(TConfiguration configuration);

    public void Reset(TConfiguration configuration);
    
    public FormFieldType FieldType { get; }
}