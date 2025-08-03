using UnityEngine;

public class GameData : MonoSingleton<GameData> 
{
    public PlayerSaveData SaveData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LoadOrInitialize();
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
                CurrentScene = "StartArea",
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
