using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : DestructibleResource, IStationary
{
    public int OverlapGroupIndex{get; set;} = -1;

    public int StationaryGroupIndex{get; set;} = -1;

    public Collider2D SpriteOverlapCollider{get; set;}

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
