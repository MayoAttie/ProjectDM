
using System;

public abstract class DataSingleton<T> where T : BaseDataScript, new()
{
    private static readonly Lazy<T> _instance = new(() =>
    {
        var obj = new T();
        (obj as IDataSingleton)?.Initialize();
        return obj;
    });

    public static T Instance => _instance.Value;
}
