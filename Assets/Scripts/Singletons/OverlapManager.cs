using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapManager : MonoBehaviour
{
    public static OverlapManager instance;

    public ContactFilter2D spriteOverlapFilter;

    public List<List<GameObject>> overlapGroups;
    public Stack<int> emptyGroupIndicies;

    public int defaultUnitOrder;

    // public LayerMask spriteOverlapLayermask;

    void Awake(){
        if(instance != this && instance != null){
            Destroy(this);
        }
        else{
            instance = this;
        }
        overlapGroups = new List<List<GameObject>>(15);
        emptyGroupIndicies = new Stack<int>(overlapGroups.Capacity);
        for(int i = 0; i < overlapGroups.Capacity; i++){
            emptyGroupIndicies.Push(i);
        }

    }

    private void LateUpdate(){

    }
}
