using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ESceneType;

/// <summary>
/// 씬 로딩과 언로딩을 관리하는 전역 매니저
/// - E_SceneType <-> 씬 이름 간 매핑 제공
/// - UniTask 기반 비동기 로드/언로드 지원
/// - 진행률 및 취소 토큰 지원
/// </summary>
public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
    // ------------------------------
    //  매핑 테이블
    // ------------------------------

    /// <summary>
    /// 씬 타입 → 씬 이름 매핑 (빌드 세팅에 등록된 이름)
    /// </summary>
    private static readonly Dictionary<E_SceneType, string> _typeToName = new()
    {
        { E_SceneType.LOADING,   "Scene_Loading"   },
        { E_SceneType.LOBBY,     "Scene_Lobby"     },
        { E_SceneType.MAIN_GAME, "Scene_MainGame"  },
    };

    /// <summary>
    /// 씬 이름 → 씬 타입 매핑
    /// </summary>
    private static readonly Dictionary<string, E_SceneType> _nameToType = new(StringComparer.Ordinal)
    {
        { "Scene_Loading",   E_SceneType.LOADING   },
        { "Scene_Lobby",     E_SceneType.LOBBY     },
        { "Scene_MainGame",  E_SceneType.MAIN_GAME },
    };

    // ------------------------------
    //  현재 씬 정보
    // ------------------------------

    /// <summary>
    /// 현재 활성 씬의 E_SceneType 반환
    /// </summary>
    public E_SceneType GetCurrentScene()
    {
        var name = SceneManager.GetActiveScene().name;
        if (_nameToType.TryGetValue(name, out var t)) return t;

        Debug.LogError($"[SceneLoadManager] Unknown scene type for: {name}");
        return E_SceneType.NONE;
    }

    // ------------------------------
    //  씬 로드 (E_SceneType 버전)
    // ------------------------------

    /// <summary>
    /// E_SceneType 기준으로 씬 로드
    /// </summary>
    /// <param name="sceneType">로드할 씬 타입</param>
    /// <param name="mode">로드 모드 (Single / Additive)</param>
    /// <param name="progress">진행률 콜백</param>
    /// <param name="ct">취소 토큰</param>
    public async UniTask LoadSceneAsync(
        E_SceneType sceneType,
        LoadSceneMode mode = LoadSceneMode.Single,
        IProgress<float> progress = null,
        CancellationToken ct = default)
    {
        var sceneName = ConvertSceneName(sceneType); // throws if invalid
        await LoadSceneAsync(sceneName, mode, progress, ct);
    }

    // ------------------------------
    //  씬 로드 (string 버전)
    // ------------------------------

    /// <summary>
    /// 씬 이름 기준으로 씬 로드
    /// </summary>
    /// <param name="sceneName">로드할 씬 이름 (빌드 세팅 등록 필수)</param>
    /// <param name="mode">로드 모드 (Single / Additive)</param>
    /// <param name="progress">진행률 콜백</param>
    /// <param name="ct">취소 토큰</param>
    /// <exception cref="ArgumentException">sceneName이 null/empty일 경우</exception>
    /// <exception cref="InvalidOperationException">씬이 빌드 세팅에 없을 경우</exception>
    public async UniTask LoadSceneAsync(
        string sceneName,
        LoadSceneMode mode = LoadSceneMode.Single,
        IProgress<float> progress = null,
        CancellationToken ct = default)
    {
        // 유효성 검사
        if (string.IsNullOrEmpty(sceneName))
            throw new ArgumentException("sceneName is null or empty.");

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
            throw new InvalidOperationException($"Scene not in Build Settings: {sceneName}");

        // 씬 로드 시작
        var op = SceneManager.LoadSceneAsync(sceneName, mode);
        op.allowSceneActivation = true;

        // AsyncOperation → UniTask 변환 + 진행률 보고
        await op.ToUniTask(
            progress: progress,              // 0.0 ~ 0.9 (Unity 규약)
            cancellationToken: ct
        );

        // Additive 모드면 로드 후 활성 씬 지정
        if (mode == LoadSceneMode.Additive)
        {
            var loaded = SceneManager.GetSceneByName(sceneName);
            if (loaded.IsValid())
                SceneManager.SetActiveScene(loaded);
        }

        // 씬 내 오브젝트 Awake/Start 실행 보장
        await UniTask.NextFrame(PlayerLoopTiming.Update, ct);
    }

    // ------------------------------
    //  씬 언로드
    // ------------------------------

    /// <summary>
    /// 씬 이름 기준 언로드
    /// </summary>
    public async UniTask UnloadSceneAsync(string sceneName, CancellationToken ct = default)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid()) return;

        var op = SceneManager.UnloadSceneAsync(scene);
        if (op != null) await op.ToUniTask(cancellationToken: ct);
    }

    // ------------------------------
    //  변환 유틸
    // ------------------------------

    /// <summary>
    /// 씬 타입 → 씬 이름 변환
    /// </summary>
    public string ConvertSceneName(E_SceneType type)
    {
        if (_typeToName.TryGetValue(type, out var name))
            return name;

        throw new ArgumentOutOfRangeException(nameof(type), type, "Unmapped scene type.");
    }

    /// <summary>
    /// 씬 이름 → 씬 타입 변환
    /// </summary>
    public E_SceneType ConvertSceneType(string name)
    {
        return _nameToType.TryGetValue(name, out var t) ? t : E_SceneType.NONE;
    }
}
