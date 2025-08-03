using Cysharp.Threading.Tasks;
using UnityEngine;

public static class PoolingUtil
{
    public static async UniTask AutoReturn<T>(this IPool<T> pool, T obj, float delaySec)
        where T : Component, IPoolable
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delaySec), DelayType.DeltaTime, PlayerLoopTiming.Update);

        if (obj != null && pool != null)
        {
            pool.Return(obj);
        }
    }
}
