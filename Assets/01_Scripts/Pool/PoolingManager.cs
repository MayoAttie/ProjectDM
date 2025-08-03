using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoSingleton<PoolingManager>
{
    private readonly Dictionary<EPooledObjectType, IPool<MonoBehaviourExtension>> _pools = new();


    /// <summary>
    /// Ǯ�� ����Ѵ�. EPooledObjectType�� Ű�� prefab�� ������� �� Ǯ�� �����Ѵ�.
    /// </summary>
    /// <param name="type">����� Ǯ Ÿ��(enum)</param>
    /// <param name="prefab">Ǯ���� ������(MonoBehaviour, IPoolable)</param>
    public void Register(EPooledObjectType type, MonoBehaviourExtension prefab)
    {
        if (_pools.ContainsKey(type)) 
            return;

        var poolGO = new GameObject($"[Pool:{type}]");
        poolGO.transform.SetParent(transform);

        var method = typeof(PoolingManager).GetMethod(nameof(CreatePool), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var generic = method.MakeGenericMethod(prefab.GetType());
        var pool = (IPool<MonoBehaviourExtension>)generic.Invoke(this, new object[] { prefab, poolGO.transform });

        _pools[type] = pool;
    }


    /// <summary>
    /// ���׸� Ÿ�� ��� Ǯ ����. PoolingMonoBehaviourExtension<T>�� �����ϰ� �ʱ�ȭ.
    /// </summary>
    /// <typeparam name="T">Ǯ���� ��ü Ÿ��</typeparam>
    /// <param name="prefab">������</param>
    /// <param name="parent">�θ� Ʈ������</param>
    /// <returns>������ Ǯ</returns>
    private IPool<MonoBehaviourExtension> CreatePool<T>(T prefab, Transform parent) where T : MonoBehaviourExtension, IPoolable
    {
        var pool = parent.gameObject.AddComponent<PoolingMonoBehaviourExtension<T>>();
        pool.Initialize(prefab);
        return pool as IPool<MonoBehaviourExtension>;
    }

    /// <summary>
    /// Ư�� Ÿ���� Ǯ���� ��ü�� �����´�.
    /// </summary>
    /// <typeparam name="T">��ü Ÿ��</typeparam>
    /// <param name="type">Ǯ Ÿ��</param>
    public T Get<T>(EPooledObjectType type) where T : MonoBehaviourExtension, IPoolable
    {
        if (_pools.TryGetValue(type, out var pool))
            return pool.Get() as T;

        Debug.LogError($"No pool registered for type {type}");
        return null;
    }

    /// <summary>
    /// Ư�� Ÿ���� Ǯ�� ��ü�� ��ȯ�Ѵ�.
    /// </summary>
    /// <typeparam name="T">��ü Ÿ��</typeparam>
    /// <param name="type">Ǯ Ÿ��</param>
    /// <param name="obj">��ȯ�� ��ü</param>
    public void Return<T>(EPooledObjectType type, T obj) where T : MonoBehaviourExtension, IPoolable
    {
        if (_pools.TryGetValue(type, out var pool))
            pool.Return(obj);
        else
            Debug.LogError($"No pool registered for type {type}");
    }

    /// <summary>
    /// Ư�� Ÿ���� Ǯ�� ��ü�� ��ϵǾ� �ִ��� Ȯ���Ѵ�.
    /// </summary>
    /// <param name="type">��ü Ÿ��</param>
    /// <returns></returns>
    public bool IsRegistered(EPooledObjectType type)
    {
        return _pools.ContainsKey(type);
    }

    /// <summary>
    /// �̸� ��ü���� ������ Ǯ�� �־�δ� �޼���
    /// </summary>
    /// <param name="type">��ü Ÿ��</param>
    /// <param name="count">������ ����</param>
    public void Preload(EPooledObjectType type, int count)
    {
        if (_pools.TryGetValue(type, out var pool))
        {
            for (int i = 0; i < count; i++)
            {
                var obj = pool.Get();
                pool.Return(obj);
            }
        }
    }

    /// <summary>
    /// ��� Ǯ�� �ִ� ��ü���� ��ȯ�Ѵ�. IPoolExtras�� ������ Ǯ������ �����Ѵ�.
    /// </summary>
    public void ReturnAll()
    {
        foreach (var pool in _pools.Values)
        {
            if (pool is IPoolExtras poolEX)
            {
                poolEX.ReturnAll();
            }
        }
    }

    /// <summary>
    /// Ư�� Ÿ���� Ǯ�� ����. IPoolExtras�� ������ Ǯ������ �����Ѵ�.
    /// </summary>
    /// <param name="type">��ü Ÿ��</param>
    public void Clear(EPooledObjectType type)
    {
        if (_pools.TryGetValue(type, out var pool) && pool is IPoolExtras extras)
        {
            extras.Clear();
        }
    }


}
