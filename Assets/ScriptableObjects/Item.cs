using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public enum ItemType{
        Axe,
        Pickaxe,
        Unique
    }
    public int id;
    public string displayName;
    public ItemType itemType;
}
