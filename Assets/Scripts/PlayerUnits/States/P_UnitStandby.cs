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
         /*
           Find the one PlayerUnitAction for each action type that involves a target object
           with the highest render order.

           The best way to do this is probably to loop thorugh the commandClickObjects and select an object of each
           type (tree, rock, enemy, etc...) -- because they will generate the same types of actions -- with highest render order.
           Maybe in the future there could be a way to do this without having to hardcode it in for each different type (tree, rock, etc...)
        */

        // Tree object with highest render order at the mouse click position.
        GameObject treeObj = commandClickObjects[0];

        // There should also be one for rocks

        // Also ones for enemies/special enemies if any

        for(int i = 1; i < commandClickObjects.Length; i++){
            if(commandClickObjects[i].GetComponent<TreeScript>() != null && 
            commandClickObjects[i].GetComponent<SortingGroup>().sortingOrder > treeObj.GetComponent<SortingGroup>().sortingOrder)
            {
                treeObj = commandClickObjects[i];
            }
        }
        
        List<PlayerUnitAction> tempList = treeObj.GetComponent<TreeScript>().GeneratePossibleActions(Ctx);
        foreach(PlayerUnitAction action in tempList){
            actionList.Add(action);
        }
    }
}
