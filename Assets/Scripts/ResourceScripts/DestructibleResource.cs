using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DestructibleResource : MonoBehaviour, IInteractable
{
    public int health;
    public ResourceData resourceData;
    public abstract List<PlayerUnitAction> GeneratePossibleActions(PlayerUnit unit);
}
