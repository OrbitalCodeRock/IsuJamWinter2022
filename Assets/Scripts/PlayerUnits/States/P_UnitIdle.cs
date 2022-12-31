using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_UnitIdle : UnitBaseState
{
    
    public P_UnitIdle(PlayerUnit currentContext, P_UnitStateFactory p_UnitStateFactory) : base(currentContext, p_UnitStateFactory)
    {

    }

    public override void EnterState()
    {

    }
    public override void UpdateState()
    {
        //if (CheckSwitchStates()) return;
    }

    public override void FixedUpdateState()
    {

    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }
}
