using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameData : MonoSingleton<GameData> 
{
    public PlayerSaveData SaveData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LoadOrInitialize();
    }

    public async UniTask InitializeAsync()
    {
        // IO가 들어갈 수 있으니 나중에 async 확장 가능
        var loaded = SaveSystem.Load();
        if (loaded != null)
        {
            SaveData = loaded;
            Debug.Log("[GameData] 저장 데이터를 불러왔습니다.");
        }
        else
        {
            SaveData = CreateNewSaveData();
            Debug.Log("[GameData] 새로운 저장 데이터를 생성했습니다.");
        }

        // 버전 마이그레이션
        MigrateIfNeeded();

        // 정의 매핑 (아이템/능력 등)
        ResolveDefinitions();
        await UniTask.Yield();
    }


    /// <summary>
    /// 저장 데이터가 있으면 불러오고, 없으면 새로 생성
    /// </summary>
    private void LoadOrInitialize()
    {
        var loaded = SaveSystem.Load();
        if (loaded != null)
        {
            SaveData = loaded;
            Debug.Log("[GameData] 저장 데이터를 불러왔습니다.");
        }
        else
        {
            SaveData = CreateNewSaveData();
            Debug.Log("[GameData] 새로운 저장 데이터를 생성했습니다.");
        }
    }

    void ResolveDefinitions()
    {
        // 예: 인벤토리 아이템 정의 매핑
        if (SaveData?.Inventory?.ObtainedItems == null) return;
        foreach (var item in SaveData.Inventory.ObtainedItems)
        {
            item.Definition = ItemDatabase.Instance.GetDefinition(item.ItemId);
            if (item.Definition == null)
                Debug.LogWarning($"[GameData] 정의 누락: {item.ItemId}");
        }
        foreach (var item in SaveData.Inventory.EquippedItems)
        {
            item.Definition = ItemDatabase.Instance.GetDefinition(item.ItemId);
        }
    }

    void MigrateIfNeeded()
    {
        const int CURRENT = 1;
        if (SaveData.SaveVersion < CURRENT)
        {
            // TODO: 버전별 마이그레이션 로직
            SaveData.SaveVersion = CURRENT;
        }
    }

    /// <summary>
    /// 새로운 저장 데이터 생성
    /// </summary>
    public PlayerSaveData CreateNewSaveData()
    {
        return new PlayerSaveData
        {
            Stats = new PlayerStatsData
            {
                MaxHP = 100,
                CurrentHP = 100,
                Level = 1,
                NextLevelEXP = 100,
                CurrentEXP = 0,
                AttackPower = 10,
                DefensePower = 5,
            },
            WorldProgress = new WorldProgressData
            {
                CurrentScene = ESceneType.E_SceneType.LOBBY,
                PlayerPosition = Vector2.zero,
                VisitedRooms = new(),
                LastCheckpointId = "StartCheckpoint"
            },
            Story = new StoryData(),
            Quests = new QuestData(),
            Unlocks = new SystemUnlockData(),
            Inventory = new InventoryData(),
            TotalPlayTime = 0,
            SaveVersion = 1
        };
    }

    /// <summary>
    /// 저장 수행
    /// </summary>
    public void Save()
    {
        SaveSystem.Save(SaveData);
    }

    /// <summary>
    /// 저장 데이터 덮어쓰기
    /// </summary>
    public void LoadFromSave(PlayerSaveData data)
    {
        SaveData = data;
    }
}
