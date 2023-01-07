using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class P_UnitStandby : UnitBaseState
{
    public P_UnitStandby(PlayerUnit currentContext, P_UnitStateFactory p_UnitStateFactory) : base(currentContext, p_UnitStateFactory)
    {
        IsRootState = true;
    }
    public override void CalculateAvailableActions(Vector2 commandClickPosition, GameObject[] commandClickObjects)
    {
        List<PlayerUnitAction> actionList = new List<PlayerUnitAction>();
        // Should also probably add a way to move behind objects as well
        if(commandClickObjects == null){
            actionList.Add(new PlayerUnitAction(Ctx.gameObject, null, PlayerUnitAction.ActionType.Move, "Move here"));
            return;
        }
         /*
           Find the one PlayerUnitAction for each action type that involves a target object
           with the highest render order.

           The best way to do this is probably to loop thorugh the commandClickObjects and select an object of each
           type (tree, rock, enemy, etc...) -- because they will generate the same types of actions -- with highest render order.
           Maybe in the future there could be a way to do this without having to hardcode it in for each different type (tree, rock, etc...)
        */

        // Tree object with highest render order at the mouse click position.
        GameObject treeObj = null;

        // There should also be one for rocks

        // Also ones for enemies/special enemies if any

        for(int i = 0; i < commandClickObjects.Length; i++){
            if(commandClickObjects[i].GetComponent<TreeScript>() == null){
                continue;
            }
            else if(treeObj == null){
                treeObj = commandClickObjects[i];
                continue;
            }
            if(commandClickObjects[i].GetComponent<SortingGroup>().sortingOrder > treeObj.GetComponent<SortingGroup>().sortingOrder)
            {
                treeObj = commandClickObjects[i];
            }
        }
        
        if(treeObj != null){
            List<PlayerUnitAction> tempList = treeObj.GetComponent<TreeScript>().GeneratePossibleActions(Ctx);
            foreach(PlayerUnitAction action in tempList){
                actionList.Add(action);
            }
        }

        // Next task is to display selectable buttons for these actions
    }
}
