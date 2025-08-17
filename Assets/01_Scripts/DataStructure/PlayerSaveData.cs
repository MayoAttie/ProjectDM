using System;
using System.Collections.Generic;
using UnityEngine;
using static ESceneType;


[Serializable]
public class PlayerSaveData : BaseDataScript
{
    public PlayerStatsData Stats;
    public WorldProgressData WorldProgress;
    public StoryData Story;
    public QuestData Quests;
    public SystemUnlockData Unlocks;
    public InventoryData Inventory;
    public InputSettingsData InputSettings;
    public float TotalPlayTime;
    public int SaveVersion = 1;
}

[Serializable]
public class PlayerStatsData : BaseDataScript
{
    public long MaxHP;             // �ִ� ü��
    public long CurrentHP;         // ���� ü��
    public int Level;              // ���� ����
    public int NextLevelEXP;       // ���� ���������� �ʿ��� ����ġ
    public int CurrentEXP;         // ���� ������ ����ġ
    public int AttackPower;        // ���ݷ�
    public int DefensePower;       // ����
}

[Serializable]
public class WorldProgressData : BaseDataScript
{
    public E_SceneType CurrentScene;                 // ���� ��ġ�� �� �̸�
    public Vector2 PlayerPosition;              // �÷��̾� ���� ��ǥ
    public List<string> VisitedRooms = new();   // �湮�� ��/���� ID ���
    public string LastCheckpointId;             // ���������� ������ üũ����Ʈ ID
    public Dictionary<string, bool> EventFlags = new(); // ���丮/����/��� ���� �÷���

}

[Serializable]
public class StoryData : BaseDataScript
{
    public List<string> CompletedStoryEvents = new(); // �Ϸ��� �ƾ�/���丮 �̺�Ʈ ID ���

}

[Serializable]
public class QuestData : BaseDataScript
{
    public List<string> ActiveQuests = new();     // ���� ���� ����Ʈ ID ���
    public List<string> CompletedQuests = new();  // �Ϸ��� ����Ʈ ID ���

}

[Serializable]
public class SystemUnlockData : BaseDataScript
{
    public List<string> OpenedDoors = new();       // ��� �� ID ���
    public List<string> OpenedShortcuts = new();   // �ر��� ���� ID ���
    public List<string> UnlockedAbilities = new(); // �ر��� �ɷ� (ex. �̴�����, ���)

}

[Serializable]
public class InventoryData : BaseDataScript
{
    public List<ItemData> ObtainedItems = new();     // 획득한 아이템 ID 목록
    public List<ItemData> EquippedItems = new();     // 장착 중인 아이템 ID 목록

}

[Serializable]
public class InputSettingsData : BaseDataScript
{
    public Dictionary<string, string> KeyBindings = new(); // InputType.ToString() -> KeyCode.ToString()
}
