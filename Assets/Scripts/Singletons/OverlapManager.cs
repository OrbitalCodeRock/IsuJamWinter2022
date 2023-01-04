using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapManager : MonoBehaviour
{
    public static OverlapManager instance;

    public ContactFilter2D spriteOverlapFilter;

    public int defaultUnitOrder;

    // public LayerMask spriteOverlapLayermask;

    void Awake(){
        if(instance != this && instance != null){
            Destroy(this);
        }
        else{
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
