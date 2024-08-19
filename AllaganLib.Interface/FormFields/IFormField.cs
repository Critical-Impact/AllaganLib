using System.Numerics;

namespace AllaganLib.Interface.FormFields;

public interface IFormField<T>
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

    public bool HasValueSet(T configuration);

    public void Draw(T configuration, int? labelSize = null, int? inputSize = null);
}