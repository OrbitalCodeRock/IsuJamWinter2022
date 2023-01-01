using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    public GameControls controls;

    // Mouse tracking
    public Vector2 mousePosition;
    public GameObject hoveredObject;
    public GameObject lastRightClickObject;

    // Unit tracking 

    // p stands for player, to differentiate from enemy units
    public GameObject[] p_Units;
    public GameObject[] p_SelectedUnits;

    void Awake(){
        if(this != instance && instance != null){
            Destroy(this);
        }
        else{
            instance = this;
        }

        controls = new GameControls();
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
