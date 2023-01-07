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
    public Vector2 mousePosition;
    public GameObject[] hoveredObjects;

    public List<GameObject> hoveredSelectableObjects;

    public GameObject[] lastCommandClickObjects;

    public Vector2 lastCommandClickPosition;

    public LayerMask mouseHitboxLayer;

    // Unit tracking 

    // p stands for player, to differentiate from enemy units
    public List<GameObject> p_Units;
    public List<GameObject> p_SelectedUnits;

    // Object Holders
    public GameObject targetHolder;

    void Awake(){
        if(this != instance && instance != null){
            Destroy(this);
        }
        else{
            instance = this;
        }

        controls = new GameControls();
        
        controls.GeneralGameplay.Select.performed += OnSelectPerformed;
        controls.GeneralGameplay.Command.performed += OnCommandPerformed;

        controls.GeneralGameplay.Enable();

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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, 0.25f, mouseHitboxLayer.value);
        if(colliders.Length > 0){
            int length = colliders.Length;
            hoveredObjects = new GameObject[length];
            hoveredSelectableObjects = new List<GameObject>(length);
            for(int i = 0; i < length; i++){
                hoveredObjects[i] = colliders[i].transform.parent.gameObject;
                if(hoveredObjects[i].GetComponent<ISelectable>() != null){
                    hoveredSelectableObjects.Add(hoveredObjects[i]);
                }
            }
        }
        else{
            hoveredObjects = null;
            hoveredSelectableObjects = null;
        }
        
    }

    private void OnCommandPerformed(InputAction.CallbackContext args){
        lastCommandClickPosition = mousePosition;
        lastCommandClickObjects = hoveredObjects;
        if(p_SelectedUnits.Count == 1){
            p_SelectedUnits[0].GetComponent<PlayerUnit>().GenerateCommandOptions(lastCommandClickPosition, lastCommandClickObjects);
        }
        else if(p_SelectedUnits.Count > 1){
            IssueGroupCommand(p_SelectedUnits);
        }
    }

    GameObject closestObject;

    private void OnSelectPerformed(InputAction.CallbackContext args){
        // GameObject[] possibleSelections = hoveredSelectableObjects.ToArray();
        // Vector2 lastMousePosition = mousePosition;
        Debug.Log("SelectPerformed");
        if(hoveredSelectableObjects == null) return;
        Debug.Log("Select phase 2");
        closestObject = hoveredSelectableObjects[0];
        GameObject p_closestUnit = null;
        foreach(GameObject selection in hoveredSelectableObjects){
            if(IsCloserToPosition(selection, closestObject, mousePosition)){
                closestObject = selection;
            }
            if(selection.GetComponent<PlayerUnit>() != null){
                if(p_closestUnit == null || IsCloserToPosition(selection,p_closestUnit,mousePosition)){
                    p_closestUnit = selection;
                }
            }
        }
        if(p_closestUnit != null){
            DeselectObjects();
            p_closestUnit.GetComponent<ISelectable>().Select();
            p_SelectedUnits = new List<GameObject>();
            p_SelectedUnits.Add(p_closestUnit);
        }
        else{
            DeselectObjects();
            if(closestObject != null){
                closestObject.GetComponent<ISelectable>().Select();
            }
        }

    }

    private void DeselectObjects(){
        foreach(GameObject unit in p_SelectedUnits){
                unit.GetComponent<ISelectable>().Deselect();
        }
        if(closestObject != null) closestObject.GetComponent<ISelectable>().Deselect();
    }

    // Returns true if a is closer or equidistant to a given position, compared to b
    private bool IsCloserToPosition(GameObject a, GameObject b, Vector2 position){
        return ((Vector2)a.transform.position - position).sqrMagnitude <= ((Vector2)b.transform.position - position).sqrMagnitude;
    }

    private void IssueGroupCommand(List<GameObject> selectedUnits){

    }
}
