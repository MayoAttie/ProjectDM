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
        // IO�� �� �� ������ ���߿� async Ȯ�� ����
        var loaded = SaveSystem.Load();
        if (loaded != null)
        {
            SaveData = loaded;
            Debug.Log("[GameData] ���� �����͸� �ҷ��Խ��ϴ�.");
        }
        else
        {
            SaveData = CreateNewSaveData();
            Debug.Log("[GameData] ���ο� ���� �����͸� �����߽��ϴ�.");
        }

        // ���� ���̱׷��̼�
        MigrateIfNeeded();

        // ���� ���� (������/�ɷ� ��)
        ResolveDefinitions();
        await UniTask.Yield();
    }


    /// <summary>
    /// ���� �����Ͱ� ������ �ҷ�����, ������ ���� ����
    /// </summary>
    private void LoadOrInitialize()
    {
        var loaded = SaveSystem.Load();
        if (loaded != null)
        {
            SaveData = loaded;
            Debug.Log("[GameData] ���� �����͸� �ҷ��Խ��ϴ�.");
        }
        else
        {
            SaveData = CreateNewSaveData();
            Debug.Log("[GameData] ���ο� ���� �����͸� �����߽��ϴ�.");
        }
    }

    void ResolveDefinitions()
    {
        // ��: �κ��丮 ������ ���� ����
        if (SaveData?.Inventory?.ObtainedItems == null) return;
        foreach (var item in SaveData.Inventory.ObtainedItems)
        {
            item.Definition = ItemDatabase.Instance.GetDefinition(item.ItemId);
            if (item.Definition == null)
                Debug.LogWarning($"[GameData] ���� ����: {item.ItemId}");
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
            // TODO: ������ ���̱׷��̼� ����
            SaveData.SaveVersion = CURRENT;
        }
    }

    /// <summary>
    /// ���ο� ���� ������ ����
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
    /// ���� ����
    /// </summary>
    public void Save()
    {
        SaveSystem.Save(SaveData);
    }

    /// <summary>
    /// ���� ������ �����
    /// </summary>
    public void LoadFromSave(PlayerSaveData data)
    {
        SaveData = data;
    }
}
