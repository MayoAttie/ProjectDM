using System;

[Serializable]
public class ItemData
{
    public string ItemId;              // 매칭 키 (SO 기준)
    public int Quantity;
    public int Durability;
    public bool IsEquipped;
    public string EquippedSlot;

    [NonSerialized] public ItemDefinition Definition; // 런타임 연결용, 저장 제외
}
