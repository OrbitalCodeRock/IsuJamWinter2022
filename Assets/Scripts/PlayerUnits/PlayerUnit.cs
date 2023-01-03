using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour, ISelectable
{

    // Skill Levels
    public int miningLvl;
    public int woodcuttingLvl;

    // Weapon Proficencies
    public int spearLvl;
    public int swordLvl;


    // Unit State Machine
    private UnitBaseState _currentState;
    private P_UnitStateFactory _states;
    public UnitBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    public Transform moveTarget;

    void Awake(){
        _states = new P_UnitStateFactory(this);
        _currentState = _states.Standby();
        _currentState.EnterState();
    }
    void Start(){
        GameManager.instance.p_Units.Add(this.gameObject);
        GameObject target = new GameObject();
        target.name = "MoveTarget";
        moveTarget = target.transform;
        moveTarget.SetParent(GameManager.instance.targetHolder.transform);
    }
     // Update is called once per frame
    void Update()
    {
        _currentState.CallUpdateStates();        
    }

    private void FixedUpdate()
    {
        _currentState.CallFixedUpdateStates();
    }

    public void GenerateCommandOptions(Vector2 commandClickPosition, GameObject[] hoveredObjects){
        // Return a list of actions available to the unit
        _currentState.CalculateAvailableActions(commandClickPosition, hoveredObjects);
    }

    public void Select(){

    }

    public void Deselect(){

    }
}
