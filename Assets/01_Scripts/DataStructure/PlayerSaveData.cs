using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerSaveData : BaseDataScript
{
    public PlayerStatsData Stats;
    public WorldProgressData WorldProgress;
    public StoryData Story;
    public QuestData Quests;
    public SystemUnlockData Unlocks;
    public InventoryData Inventory;
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
    public string CurrentScene;                 // ���� ��ġ�� �� �̸�
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
    public List<ItemData> ObtainedItems = new();     // ȹ���� ������ ID ���
    public List<ItemData> EquippedItems = new();     // ���� ���� ��� ID ���

}
