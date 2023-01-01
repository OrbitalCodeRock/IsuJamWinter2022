using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_UnitMoving : UnitBaseState
{
    public Vector2? targetPosition;
    public P_UnitMoving(PlayerUnit currentContext, P_UnitStateFactory p_UnitStateFactory) : base(currentContext, p_UnitStateFactory)
    {
    
    }
    public override void UpdateState(){
        if(targetPosition != null){
            
        }
    }
}
