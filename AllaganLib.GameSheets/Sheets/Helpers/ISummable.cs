namespace AllaganLib.GameSheets.Sheets.Helpers;

public interface ISummable<T>
{
    T Add(T a, T b);
}