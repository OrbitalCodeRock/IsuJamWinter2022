using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitAction
{
    public PlayerUnitAction(GameObject Actor, GameObject targetObject, ActionType actionType, string displayName){
        this.Actor = Actor;
        this.targetObject = targetObject;
        this.actionType = actionType;
        this.displayName = displayName;
    }
    public enum ActionType{
        Move,
        Chop,
        Mine,
        Attack,
    }

    public GameObject Actor;
    public GameObject targetObject;
    public ActionType actionType;
    public string displayName;

}
