using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_UnitStateFactory : MonoBehaviour
{
    private PlayerUnit _context;

    private enum PlayerStates
    {
        idle,
        moving,
        attacking,
        gathering,
        standby,
    }

    Dictionary<PlayerStates, UnitBaseState> _states = new Dictionary<PlayerStates, UnitBaseState>();

    public P_UnitStateFactory(PlayerUnit currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.idle] = new P_UnitIdle(_context, this);
        _states[PlayerStates.moving] = new P_UnitMoving(_context, this);
        _states[PlayerStates.attacking] = new P_UnitAttacking(_context, this);
        _states[PlayerStates.gathering] = new P_UnitGathering(_context, this);
        _states[PlayerStates.standby] = new P_UnitStandby(_context, this);

    }

    public UnitBaseState Idle()
    {
        return _states[PlayerStates.idle];
    }

    public UnitBaseState Moving()
    {
        return _states[PlayerStates.moving];
    }

    public UnitBaseState Attacking()
    {
        return _states[PlayerStates.attacking];
    }

    public UnitBaseState Gathering()
    {
        return _states[PlayerStates.gathering];
    }

    public UnitBaseState Standby()
    {
        return _states[PlayerStates.standby];
    }
}
