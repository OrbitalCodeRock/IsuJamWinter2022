using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    // New Input System Setup
    public GameControls controls;

    // Camera
    public Camera viewCamera;

    // Mouse tracking
    [HideInInspector]
    public Vector2 mousePosition;
    public GameObject[] hoveredObjects;

    [HideInInspector]
    public GameObject[] lastCommandClickObjects;

    [HideInInspector]
    public Vector2 lastCommandClickPosition;

    public LayerMask mouseHitboxLayer;

    // Unit tracking 

    // p stands for player, to differentiate from enemy units
    public GameObject[] p_Units;
    public GameObject[] p_SelectedUnits;

    private void OnCommandPerformed(InputAction.CallbackContext args){
        lastCommandClickPosition = mousePosition;
        lastCommandClickObjects = hoveredObjects;
    }

    void Awake(){
        if(this != instance && instance != null){
            Destroy(this);
        }
        else{
            instance = this;
        }

        controls = new GameControls();

        controls.GeneralGameplay.Command.performed += OnCommandPerformed;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = viewCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Get objects that the mouse is currently hovering over
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, 0.5f, mouseHitboxLayer.value);
        if(colliders != null){
            hoveredObjects = new GameObject[colliders.Length];
            for(int i = 0; i < colliders.Length; i++){
                hoveredObjects[i] = colliders[i].transform.parent.gameObject;
            }
        }
        else{
            hoveredObjects = null;
        }
        
    }
}
