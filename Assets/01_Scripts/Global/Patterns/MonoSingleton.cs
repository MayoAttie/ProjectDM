using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviourExtension where T : MonoBehaviourExtension
{
    private static T _instance;
    private static readonly object _lock = new();

    public static T Instance
    {
        get
        {
            if (_instance != null) 
                return _instance;

            lock (_lock)
            {
                if (_instance == null)
                {
                    var existing = FindAnyObjectByType<T>();
                    if (existing != null)
                    {
                        _instance = existing;
                    }
                    else
                    {
                        var go = new GameObject($"[{typeof(T).Name}]");
                        _instance = go.AddComponent<T>();
                        DontDestroyOnLoad(go);
                    }
                }
                return 
                    _instance;
            }
        }
    }

    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
