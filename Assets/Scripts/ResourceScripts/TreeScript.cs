using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : DestructibleResource, IStationary
{
    public int OverlapGroupIndex{get; set;} = -1;

    public int StationaryGroupIndex{get; set;} = -1;

    public Collider2D spriteOverlapCollider;
    public Collider2D SpriteOverlapCollider{get; set;}

    public void Awake(){
        SpriteOverlapCollider = spriteOverlapCollider;
    }
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

    /* I should probably allow stationary objects to add and remove themselves to overlap groups and stationary groups during
       OnCollisionEnter and OnCollisionEnter. I could also check to make the current stationary object transparent. 
       To do this, I should probably do a collider cast and turn this object transparent if there is a non-stationary
       overlappable object with a minimum bounding box height higher than this object's minimum bounding box height. */
}