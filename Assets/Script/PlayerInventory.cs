using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Weapon_Type,
    Ability_Type,
    NON,
}
public struct Slot
{
    private string name;
    private int level;
    private int cost;

    public void SetData(Slot itemSlot)
    {
        name = itemSlot.name;
        level = itemSlot.level;
        cost = itemSlot.cost;
    }
}

public class PlayerInventory
{
    Dictionary<ItemType,List<Slot>> slotDic;
    int index = 0;
    public int money { get; private set; }
    public void SetItemData(ItemType type, Slot item_slot)
    {
        Slot slot = new Slot();
        slot.SetData(item_slot);
        //slotDic.Add(type, slot);
    }

    //public Slot SetSlot(ItemType type, Slot item_slot)
    //{
    //    return slotDic[type];
    //}
    public void EarnMoney(int cost) => money += cost;
}
