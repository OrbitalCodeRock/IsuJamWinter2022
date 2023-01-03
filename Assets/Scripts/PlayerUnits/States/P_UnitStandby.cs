using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_UnitStandby : UnitBaseState
{
    public P_UnitStandby(PlayerUnit currentContext, P_UnitStateFactory p_UnitStateFactory) : base(currentContext, p_UnitStateFactory)
    {
        IsRootState = true;
    }
    public override void CalculateAvailableActions(Vector2 commandClickPosition, GameObject[] hoveredObjects)
    {
        
    }
}
