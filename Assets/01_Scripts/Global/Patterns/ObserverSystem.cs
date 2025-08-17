using Project.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

// 정적 이벤트 매니저
public static class ObserverSystem
{
    private static Dictionary<System.Type, HashSet<IObserverSubscribe>> eventListeners
        = new Dictionary<System.Type, HashSet<IObserverSubscribe>>();

    // 특정 타입의 이벤트에 구독
    public static void Subscribe<T>(IObserverSubscribe observer) where T : IObservable
    {
        System.Type eventType = typeof(T);

        if (!eventListeners.ContainsKey(eventType))
            eventListeners[eventType] = new HashSet<IObserverSubscribe>();

        eventListeners[eventType].Add(observer);

#if UNITY_EDITOR
        DebugLog.Log($"Observer subscribed to {eventType.Name}. Total listeners: {eventListeners[eventType].Count}");
#endif
    }

    // 특정 타입의 이벤트에서 구독 해제
    public static void Unsubscribe<T>(IObserverSubscribe observer) where T : IObservable
    {
        System.Type eventType = typeof(T);

        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType].Remove(observer);

            // 리스너가 없으면 딕셔너리에서 제거
            if (eventListeners[eventType].Count == 0)
                eventListeners.Remove(eventType);

#if UNITY_EDITOR
            DebugLog.Log($"Observer unsubscribed from {eventType.Name}");
#endif
        }
    }

    // 모든 이벤트에서 구독 해제 (MonoBehaviour가 파괴될 때 유용)
    public static void UnsubscribeAll(IObserverSubscribe observer)
    {
        var keysToRemove = new List<System.Type>();

        foreach (var kvp in eventListeners)
        {
            kvp.Value.Remove(observer);
            if (kvp.Value.Count == 0)
                keysToRemove.Add(kvp.Key);
        }

        // 빈 리스너 딕셔너리 제거
        foreach (var key in keysToRemove)
        {
            eventListeners.Remove(key);
        }
    }

    // 이벤트 발생
    public static void Notify<T>(T observable, object data = null) where T : IObservable
    {
        System.Type eventType = typeof(T);

        if (!eventListeners.ContainsKey(eventType))
            return;

        // 리스너 복사본 생성 (iteration 중 수정 방지)
        var listeners = new HashSet<IObserverSubscribe>(eventListeners[eventType]);

        foreach (var listener in listeners)
        {
            try
            {
                // Unity Object가 파괴되었는지 확인
                if (listener is MonoBehaviour mb && mb == null)
                {
                    eventListeners[eventType].Remove(listener);
                    continue;
                }

                listener.OnNotify(observable, data);
            }
            catch (Exception e)
            {
                DebugLog.Error($"Error notifying observer for {eventType.Name}: {e.Message}");
            }
        }
    }

    // 디버깅용: 현재 등록된 리스너 수 출력
    public static void LogListenerCounts()
    {
#if UNITY_EDITOR
        DebugLog.Log("=== Event Manager Status ===");
        foreach (var kvp in eventListeners)
        {
            DebugLog.Log($"{kvp.Key.Name}: {kvp.Value.Count} listeners");
        }
#endif
    }

    // 메모리 정리 (씬 전환 시 호출하면 좋음)
    public static void Clear()
    {
        eventListeners.Clear();
        DebugLog.Log("EventManager cleared all listeners");
    }
}