using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDatabase : MonoSingleton<ItemDatabase>
{
    [SerializeField]
    private List<ItemDefinition> itemDefinitions; // 인스펙터 수동 등록용

    private Dictionary<string, ItemDefinition> itemMap;

    protected override void Awake()
    {
        base.Awake();
        LoadDefinitions();
    }

    private void LoadDefinitions()
    {
        // 수동 등록 + 자동 로드 혼합 가능
        var loadedFromResources = Resources.LoadAll<ItemDefinition>("Data/Items");

        // 중복 방지 및 병합
        var all = new HashSet<ItemDefinition>(itemDefinitions);
        foreach (var def in loadedFromResources)
            all.Add(def);

        itemDefinitions = all.ToList();
        itemMap = itemDefinitions.ToDictionary(d => d.ItemId, d => d);

        Debug.Log($"[ItemDatabase] {itemMap.Count}개 아이템 정의 로드 완료");
    }

    public ItemDefinition GetDefinition(string itemId)
    {
        if (itemMap.TryGetValue(itemId, out var def))
            return def;
        Debug.LogWarning($"[ItemDatabase] 정의를 찾을 수 없음: {itemId}");
        return null;
    }

    public IEnumerable<ItemDefinition> GetByType(ItemDefinition.ItemType type)
    {
        return itemDefinitions.Where(d => d.Type == type);
    }
}
