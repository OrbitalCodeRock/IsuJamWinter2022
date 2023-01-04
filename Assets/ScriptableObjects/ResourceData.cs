using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Resources/ResourceData", order = 1)]
public class ResourceData : ScriptableObject
{
    public enum ResourceType{
        Tree,
        Rock,
        Plant,
    }
    public int maxHealth;
    public ResourceType dropType;

    /* This will probably be changed in the future to make use of some sort of "drop table"
       with drops possibly containing multiple different resources, and each drop having it's own probability */
    public int dropAmount;
}
