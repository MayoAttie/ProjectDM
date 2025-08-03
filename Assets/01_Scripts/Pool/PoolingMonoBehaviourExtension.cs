using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class PoolingMonoBehaviourExtension<T> : MonoBehaviourExtension, IPool<T>, IPoolExtras where T : Component, IPoolable
{
    private readonly Stack<T> pool = new ();
    private T prefab;
    public T Get()
    {
        T item;
        if (pool.Count > 0)
        {
            item = pool.Pop();
        }
        else
        {
            item = Instantiate(prefab, transform);
        }

        item.gameObject.SetActive(true);
        item.OnGetFromPool();
        return item;
    }

    public void Return(T item)
    {
        if (item == null)
        {
            Debug.LogWarning("Attempted to return a null item to the pool.");
            return;
        }
        item.OnReturnToPool();
        item.gameObject.SetActive(false);
        item.transform.SetParent(transform);
        pool.Push(item);
    }

    public void Initialize(T prefab)
    {
        this.prefab = prefab;
    }


    public void ReturnAll()
    {
        foreach (var item in pool)
        {
            if (item != null)
                item.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        foreach (var item in pool)
        {
            if (item != null)
            {
                GameObject.Destroy(item.gameObject);
            }
        }
        pool.Clear();
    }

    public int Count => pool.Count;

}
