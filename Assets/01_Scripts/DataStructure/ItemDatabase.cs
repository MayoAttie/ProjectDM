using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDatabase : MonoSingleton<ItemDatabase>
{
    [SerializeField]
    private List<ItemDefinition> itemDefinitions; // �ν����� ���� ��Ͽ�

    private Dictionary<string, ItemDefinition> itemMap;

    protected override void Awake()
    {
        base.Awake();
        LoadDefinitions();
    }

    private void LoadDefinitions()
    {
        // ���� ��� + �ڵ� �ε� ȥ�� ����
        var loadedFromResources = Resources.LoadAll<ItemDefinition>("Data/Items");

        // �ߺ� ���� �� ����
        var all = new HashSet<ItemDefinition>(itemDefinitions);
        foreach (var def in loadedFromResources)
            all.Add(def);

        itemDefinitions = all.ToList();
        itemMap = itemDefinitions.ToDictionary(d => d.ItemId, d => d);

        Debug.Log($"[ItemDatabase] {itemMap.Count}�� ������ ���� �ε� �Ϸ�");
    }

    public ItemDefinition GetDefinition(string itemId)
    {
        if (itemMap.TryGetValue(itemId, out var def))
            return def;
        Debug.LogWarning($"[ItemDatabase] ���Ǹ� ã�� �� ����: {itemId}");
        return null;
    }

    public IEnumerable<ItemDefinition> GetByType(ItemDefinition.ItemType type)
    {
        return itemDefinitions.Where(d => d.Type == type);
    }
}
