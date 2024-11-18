namespace AllaganLib.GameSheets.Model;

public class ItemModelHelper
{
    public string ImcFileFormat { get; private set; }

    public byte ImcPartKey { get; private set; }

    public string ModelFileFormat { get; private set; }

    public byte VariantIndexWord { get; private set; }

    public ItemModelHelper(string imcFileFormat, byte imcPartKey, string modelFileFormat, byte variantIndexWord) {
        this.ImcFileFormat = imcFileFormat;
        this.ImcPartKey = imcPartKey;
        this.ModelFileFormat = modelFileFormat;
        this.VariantIndexWord = variantIndexWord;
    }
}