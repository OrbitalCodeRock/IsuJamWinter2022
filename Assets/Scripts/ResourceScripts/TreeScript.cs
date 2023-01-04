using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : DestructibleResource
{
    public override List<PlayerUnitAction> GeneratePossibleActions(PlayerUnit unit){
        List<PlayerUnitAction> actionList = new List<PlayerUnitAction>();
        foreach(Item item in unit.carriedItems){
            if(item.itemType == Item.ItemType.Axe){
                PlayerUnitAction action = new PlayerUnitAction(unit.gameObject, this.gameObject, PlayerUnitAction.ActionType.Chop, "Chop tree");
                actionList.Add(action);
                break;
            }
        }
        return actionList;
    }
}
