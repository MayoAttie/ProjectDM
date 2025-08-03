using System;

[Serializable]
public class ItemData
{
    public string ItemId;              // ��Ī Ű (SO ����)
    public int Quantity;
    public int Durability;
    public bool IsEquipped;
    public string EquippedSlot;

    [NonSerialized] public ItemDefinition Definition; // ��Ÿ�� �����, ���� ����
}
