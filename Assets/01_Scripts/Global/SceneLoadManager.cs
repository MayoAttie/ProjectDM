using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ESceneType;

/// <summary>
/// �� �ε��� ��ε��� �����ϴ� ���� �Ŵ���
/// - E_SceneType <-> �� �̸� �� ���� ����
/// - UniTask ��� �񵿱� �ε�/��ε� ����
/// - ����� �� ��� ��ū ����
/// </summary>
public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
    // ------------------------------
    //  ���� ���̺�
    // ------------------------------

    /// <summary>
    /// �� Ÿ�� �� �� �̸� ���� (���� ���ÿ� ��ϵ� �̸�)
    /// </summary>
    private static readonly Dictionary<E_SceneType, string> _typeToName = new()
    {
        { E_SceneType.LOADING,   "Scene_Loading"   },
        { E_SceneType.LOBBY,     "Scene_Lobby"     },
        { E_SceneType.MAIN_GAME, "Scene_MainGame"  },
    };

    /// <summary>
    /// �� �̸� �� �� Ÿ�� ����
    /// </summary>
    private static readonly Dictionary<string, E_SceneType> _nameToType = new(StringComparer.Ordinal)
    {
        { "Scene_Loading",   E_SceneType.LOADING   },
        { "Scene_Lobby",     E_SceneType.LOBBY     },
        { "Scene_MainGame",  E_SceneType.MAIN_GAME },
    };

    // ------------------------------
    //  ���� �� ����
    // ------------------------------

    /// <summary>
    /// ���� Ȱ�� ���� E_SceneType ��ȯ
    /// </summary>
    public E_SceneType GetCurrentScene()
    {
        var name = SceneManager.GetActiveScene().name;
        if (_nameToType.TryGetValue(name, out var t)) return t;

        Debug.LogError($"[SceneLoadManager] Unknown scene type for: {name}");
        return E_SceneType.NONE;
    }

    // ------------------------------
    //  �� �ε� (E_SceneType ����)
    // ------------------------------

    /// <summary>
    /// E_SceneType �������� �� �ε�
    /// </summary>
    /// <param name="sceneType">�ε��� �� Ÿ��</param>
    /// <param name="mode">�ε� ��� (Single / Additive)</param>
    /// <param name="progress">����� �ݹ�</param>
    /// <param name="ct">��� ��ū</param>
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
    //  �� �ε� (string ����)
    // ------------------------------

    /// <summary>
    /// �� �̸� �������� �� �ε�
    /// </summary>
    /// <param name="sceneName">�ε��� �� �̸� (���� ���� ��� �ʼ�)</param>
    /// <param name="mode">�ε� ��� (Single / Additive)</param>
    /// <param name="progress">����� �ݹ�</param>
    /// <param name="ct">��� ��ū</param>
    /// <exception cref="ArgumentException">sceneName�� null/empty�� ���</exception>
    /// <exception cref="InvalidOperationException">���� ���� ���ÿ� ���� ���</exception>
    public async UniTask LoadSceneAsync(
        string sceneName,
        LoadSceneMode mode = LoadSceneMode.Single,
        IProgress<float> progress = null,
        CancellationToken ct = default)
    {
        // ��ȿ�� �˻�
        if (string.IsNullOrEmpty(sceneName))
            throw new ArgumentException("sceneName is null or empty.");

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
            throw new InvalidOperationException($"Scene not in Build Settings: {sceneName}");

        // �� �ε� ����
        var op = SceneManager.LoadSceneAsync(sceneName, mode);
        op.allowSceneActivation = true;

        // AsyncOperation �� UniTask ��ȯ + ����� ����
        await op.ToUniTask(
            progress: progress,              // 0.0 ~ 0.9 (Unity �Ծ�)
            cancellationToken: ct
        );

        // Additive ���� �ε� �� Ȱ�� �� ����
        if (mode == LoadSceneMode.Additive)
        {
            var loaded = SceneManager.GetSceneByName(sceneName);
            if (loaded.IsValid())
                SceneManager.SetActiveScene(loaded);
        }

        // �� �� ������Ʈ Awake/Start ���� ����
        await UniTask.NextFrame(PlayerLoopTiming.Update, ct);
    }

    // ------------------------------
    //  �� ��ε�
    // ------------------------------

    /// <summary>
    /// �� �̸� ���� ��ε�
    /// </summary>
    public async UniTask UnloadSceneAsync(string sceneName, CancellationToken ct = default)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid()) return;

        var op = SceneManager.UnloadSceneAsync(scene);
        if (op != null) await op.ToUniTask(cancellationToken: ct);
    }

    // ------------------------------
    //  ��ȯ ��ƿ
    // ------------------------------

    /// <summary>
    /// �� Ÿ�� �� �� �̸� ��ȯ
    /// </summary>
    public string ConvertSceneName(E_SceneType type)
    {
        if (_typeToName.TryGetValue(type, out var name))
            return name;

        throw new ArgumentOutOfRangeException(nameof(type), type, "Unmapped scene type.");
    }

    /// <summary>
    /// �� �̸� �� �� Ÿ�� ��ȯ
    /// </summary>
    public E_SceneType ConvertSceneType(string name)
    {
        return _nameToType.TryGetValue(name, out var t) ? t : E_SceneType.NONE;
    }
}
