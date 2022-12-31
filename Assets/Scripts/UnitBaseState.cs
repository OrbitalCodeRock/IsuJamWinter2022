using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBaseState
{
    
    private bool _isRootState = false;
    private PlayerUnit _ctx;
    private P_UnitStateFactory _factory;

    private UnitBaseState _currentSubState;

    private UnitBaseState _currentSuperState;

    protected bool IsRootState {set{_isRootState = value;}}
    protected PlayerUnit Ctx{ get{return _ctx; }}
    protected P_UnitStateFactory Factory{get{return _factory;} }

    public UnitBaseState(PlayerUnit currentContext, P_UnitStateFactory p_UnitStateFactory)
    {
        _ctx = currentContext;
        _factory = p_UnitStateFactory;
    }

    public virtual void EnterState(){}
    public virtual void UpdateState(){}
    public virtual void FixedUpdateState(){}
    public virtual void ExitState(){}
    // Returns True if state switch was made, false if not
    public virtual bool CheckSwitchStates() { return false; }
    public virtual void InitializeSubState(){}

    public void CallUpdateStates()
    {
        UpdateState();
        if(_currentSubState != null){
            _currentSubState.CallUpdateStates();
        }
    }

    public void CallFixedUpdateStates()
    {
        FixedUpdateState();
        if(_currentSubState != null)
        {
            _currentSubState.CallFixedUpdateStates();
        }
    }

    public void CallExitStates(){
        ExitState();
        if(_currentSubState != null){
            _currentSubState.CallExitStates();
        }
    }

    protected void SwitchState(UnitBaseState newState)
    {

        ExitState();
        newState.EnterState();
        if(_isRootState){
            _ctx.CurrentState = newState;
        }
        else if(_currentSuperState != null){
            _currentSuperState.SetSubState(newState);
        }

    }

    protected void SetSuperState(UnitBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(UnitBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        _currentSubState.EnterState();
    }
}
