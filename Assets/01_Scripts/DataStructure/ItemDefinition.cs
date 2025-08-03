using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public string ItemId;
    public string ItemName;
    public string Description;
    public Sprite Icon;
    public ItemType Type;
    public int MaxStackSize;
    public int Rarity;
    public int BaseDurability;
    public string[] Attributes;

    public enum ItemType
    {
        Weapon, Armor, Consumable, QuestItem, Misc
    }
}
