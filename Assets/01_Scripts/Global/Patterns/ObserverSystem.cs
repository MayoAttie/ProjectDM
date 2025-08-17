using System;
using System.Collections.Generic;
using UnityEngine;

// ���� �̺�Ʈ �Ŵ���
public static class ObserverSystem
{
    private static Dictionary<System.Type, HashSet<IObserverSubscribe>> eventListeners
        = new Dictionary<System.Type, HashSet<IObserverSubscribe>>();

    // Ư�� Ÿ���� �̺�Ʈ�� ����
    public static void Subscribe<T>(IObserverSubscribe observer) where T : IObservable
    {
        System.Type eventType = typeof(T);

        if (!eventListeners.ContainsKey(eventType))
            eventListeners[eventType] = new HashSet<IObserverSubscribe>();

        eventListeners[eventType].Add(observer);

#if UNITY_EDITOR
        Debug.Log($"Observer subscribed to {eventType.Name}. Total listeners: {eventListeners[eventType].Count}");
#endif
    }

    // Ư�� Ÿ���� �̺�Ʈ���� ���� ����
    public static void Unsubscribe<T>(IObserverSubscribe observer) where T : IObservable
    {
        System.Type eventType = typeof(T);

        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType].Remove(observer);

            // �����ʰ� ������ ��ųʸ����� ����
            if (eventListeners[eventType].Count == 0)
                eventListeners.Remove(eventType);

#if UNITY_EDITOR
            Debug.Log($"Observer unsubscribed from {eventType.Name}");
#endif
        }
    }

    // ��� �̺�Ʈ���� ���� ���� (MonoBehaviour�� �ı��� �� ����)
    public static void UnsubscribeAll(IObserverSubscribe observer)
    {
        var keysToRemove = new List<System.Type>();

        foreach (var kvp in eventListeners)
        {
            kvp.Value.Remove(observer);
            if (kvp.Value.Count == 0)
                keysToRemove.Add(kvp.Key);
        }

        // �� ������ ��ųʸ� ����
        foreach (var key in keysToRemove)
        {
            eventListeners.Remove(key);
        }
    }

    // �̺�Ʈ �߻�
    public static void Notify<T>(T observable, object data = null) where T : IObservable
    {
        System.Type eventType = typeof(T);

        if (!eventListeners.ContainsKey(eventType))
            return;

        // ������ ���纻 ���� (iteration �� ���� ����)
        var listeners = new HashSet<IObserverSubscribe>(eventListeners[eventType]);

        foreach (var listener in listeners)
        {
            try
            {
                // Unity Object�� �ı��Ǿ����� Ȯ��
                if (listener is MonoBehaviour mb && mb == null)
                {
                    eventListeners[eventType].Remove(listener);
                    continue;
                }

                listener.OnNotify(observable, data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error notifying observer for {eventType.Name}: {e.Message}");
            }
        }
    }

    // ������: ���� ��ϵ� ������ �� ���
    public static void LogListenerCounts()
    {
#if UNITY_EDITOR
        Debug.Log("=== Event Manager Status ===");
        foreach (var kvp in eventListeners)
        {
            Debug.Log($"{kvp.Key.Name}: {kvp.Value.Count} listeners");
        }
#endif
    }

    // �޸� ���� (�� ��ȯ �� ȣ���ϸ� ����)
    public static void Clear()
    {
        eventListeners.Clear();
        Debug.Log("EventManager cleared all listeners");
    }
}