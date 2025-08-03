using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoSingleton<PoolingManager>
{
    private readonly Dictionary<EPooledObjectType, IPool<MonoBehaviourExtension>> _pools = new();


    /// <summary>
    /// 풀을 등록한다. EPooledObjectType을 키로 prefab을 기반으로 한 풀을 생성한다.
    /// </summary>
    /// <param name="type">등록할 풀 타입(enum)</param>
    /// <param name="prefab">풀링할 프리팹(MonoBehaviour, IPoolable)</param>
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
    /// 제네릭 타입 기반 풀 생성. PoolingMonoBehaviourExtension<T>를 생성하고 초기화.
    /// </summary>
    /// <typeparam name="T">풀링할 객체 타입</typeparam>
    /// <param name="prefab">프리팹</param>
    /// <param name="parent">부모 트랜스폼</param>
    /// <returns>생성된 풀</returns>
    private IPool<MonoBehaviourExtension> CreatePool<T>(T prefab, Transform parent) where T : MonoBehaviourExtension, IPoolable
    {
        var pool = parent.gameObject.AddComponent<PoolingMonoBehaviourExtension<T>>();
        pool.Initialize(prefab);
        return pool as IPool<MonoBehaviourExtension>;
    }

    /// <summary>
    /// 특정 타입의 풀에서 객체를 꺼내온다.
    /// </summary>
    /// <typeparam name="T">객체 타입</typeparam>
    /// <param name="type">풀 타입</param>
    public T Get<T>(EPooledObjectType type) where T : MonoBehaviourExtension, IPoolable
    {
        if (_pools.TryGetValue(type, out var pool))
            return pool.Get() as T;

        Debug.LogError($"No pool registered for type {type}");
        return null;
    }

    /// <summary>
    /// 특정 타입의 풀에 객체를 반환한다.
    /// </summary>
    /// <typeparam name="T">객체 타입</typeparam>
    /// <param name="type">풀 타입</param>
    /// <param name="obj">반환할 객체</param>
    public void Return<T>(EPooledObjectType type, T obj) where T : MonoBehaviourExtension, IPoolable
    {
        if (_pools.TryGetValue(type, out var pool))
            pool.Return(obj);
        else
            Debug.LogError($"No pool registered for type {type}");
    }

    /// <summary>
    /// 특정 타입의 풀에 객체가 등록되어 있는지 확인한다.
    /// </summary>
    /// <param name="type">객체 타입</param>
    /// <returns></returns>
    public bool IsRegistered(EPooledObjectType type)
    {
        return _pools.ContainsKey(type);
    }

    /// <summary>
    /// 미리 객체들을 생성해 풀에 넣어두는 메서드
    /// </summary>
    /// <param name="type">객체 타입</param>
    /// <param name="count">생성할 갯수</param>
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
    /// 모든 풀에 있는 객체들을 반환한다. IPoolExtras를 구현한 풀에서만 동작한다.
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
    /// 특정 타입의 풀을 비운다. IPoolExtras를 구현한 풀에서만 동작한다.
    /// </summary>
    /// <param name="type">객체 타입</param>
    public void Clear(EPooledObjectType type)
    {
        if (_pools.TryGetValue(type, out var pool) && pool is IPoolExtras extras)
        {
            extras.Clear();
        }
    }


}
