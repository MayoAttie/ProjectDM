using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoSingleton<ProjectManager>
{
    protected override void Awake()
    {
        base.Awake();
        AwakeGameSequence();
    }

    private void Start()
    {
        StartGameSequence().Forget();
    }

    private void AwakeGameSequence()
    {
        // 1) �ʼ� �̱���/�����ͺ��̽� �غ�
        _ = ItemDatabase.Instance;     // �̸� ���� + DontDestroyOnLoad
        _ = GameData.Instance;         // �̸� ������
        // �ʿ��ϸ� AbilityDatabase/Audio/Localization � ���⼭ ����
    }

    private async UniTask StartGameSequence()
    {
        // 2) �ε� UI ON (����)
        // UIManager.ShowLoading(true);

        // 3) GameData �ʱ�ȭ(�ε�/���� ����) + ���� ����
        await GameData.Instance.InitializeAsync();

        // 4) (����) �� ����/�÷��̾� ����
        await LoadSavedNowScene();

        // 5) �ε� UI OFF
        // UIManager.ShowLoading(false);
    }

    private async UniTask LoadSavedNowScene()
    {
        var wd = GameData.Instance.SaveData.WorldProgress;
        // ���� ���� ���� ���� �ٸ��� �ε�
        if (SceneLoadManager.Instance.GetCurrentScene() != wd.CurrentScene)
        {
            await SceneLoadManager.Instance.LoadSceneAsync(wd.CurrentScene, LoadSceneMode.Single);
        }

        // TODO : ����/��ġ ����
        //PlayerSpawner.SpawnAt(wd.PlayerPosition, wd.LastCheckpointId);
    }
}
