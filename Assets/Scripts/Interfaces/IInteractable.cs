using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public List<PlayerUnitAction> GeneratePossibleActions(PlayerUnit unit);
}
