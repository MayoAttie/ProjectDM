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
        // 1) 필수 싱글턴/데이터베이스 준비
        _ = ItemDatabase.Instance;     // 미리 생성 + DontDestroyOnLoad
        _ = GameData.Instance;         // 미리 생성만
        // 필요하면 AbilityDatabase/Audio/Localization 등도 여기서 보장
    }

    private async UniTask StartGameSequence()
    {
        // 2) 로딩 UI ON (선택)
        // UIManager.ShowLoading(true);

        // 3) GameData 초기화(로드/새로 생성) + 정의 매핑
        await GameData.Instance.InitializeAsync();

        // 4) (선택) 씬 진입/플레이어 스폰
        await LoadSavedNowScene();

        // 5) 로딩 UI OFF
        // UIManager.ShowLoading(false);
    }

    private async UniTask LoadSavedNowScene()
    {
        var wd = GameData.Instance.SaveData.WorldProgress;
        // 현재 씬과 저장 씬이 다르면 로드
        if (SceneLoadManager.Instance.GetCurrentScene() != wd.CurrentScene)
        {
            await SceneLoadManager.Instance.LoadSceneAsync(wd.CurrentScene, LoadSceneMode.Single);
        }

        // TODO : 스폰/위치 복원
        //PlayerSpawner.SpawnAt(wd.PlayerPosition, wd.LastCheckpointId);
    }
}
