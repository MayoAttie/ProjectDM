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
    public long MaxHP;             // 최대 체력
    public long CurrentHP;         // 현재 체력
    public int Level;              // 현재 레벨
    public int NextLevelEXP;       // 다음 레벨업까지 필요한 경험치
    public int CurrentEXP;         // 현재 보유한 경험치
    public int AttackPower;        // 공격력
    public int DefensePower;       // 방어력
}

[Serializable]
public class WorldProgressData : BaseDataScript
{
    public string CurrentScene;                 // 현재 위치한 씬 이름
    public Vector2 PlayerPosition;              // 플레이어 현재 좌표
    public List<string> VisitedRooms = new();   // 방문한 방/구역 ID 목록
    public string LastCheckpointId;             // 마지막으로 도달한 체크포인트 ID
    public Dictionary<string, bool> EventFlags = new(); // 스토리/연출/기믹 조건 플래그

}

[Serializable]
public class StoryData : BaseDataScript
{
    public List<string> CompletedStoryEvents = new(); // 완료한 컷씬/스토리 이벤트 ID 목록

}

[Serializable]
public class QuestData : BaseDataScript
{
    public List<string> ActiveQuests = new();     // 진행 중인 퀘스트 ID 목록
    public List<string> CompletedQuests = new();  // 완료한 퀘스트 ID 목록

}

[Serializable]
public class SystemUnlockData : BaseDataScript
{
    public List<string> OpenedDoors = new();       // 열어본 문 ID 목록
    public List<string> OpenedShortcuts = new();   // 해금한 숏컷 ID 목록
    public List<string> UnlockedAbilities = new(); // 해금한 능력 (ex. 이단점프, 대시)

}

[Serializable]
public class InventoryData : BaseDataScript
{
    public List<ItemData> ObtainedItems = new();     // 획득한 아이템 ID 목록
    public List<ItemData> EquippedItems = new();     // 장착 중인 장비 ID 목록

}
