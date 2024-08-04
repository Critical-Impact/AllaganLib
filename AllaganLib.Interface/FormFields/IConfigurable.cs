namespace AllaganLib.Interface.FormFields;

public interface IConfigurable<T>
{
    public T? Get(string key);

    public void Set(string key, T? newValue);
}