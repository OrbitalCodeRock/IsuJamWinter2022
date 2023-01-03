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

    private void OnSelectPerformed(InputAction.CallbackContext args){
        GameObject[] possibleSelections = hoveredObjects;
        Vector2 lastMousePosition = mousePosition;
        GameObject closestObject = possibleSelections[0];
        GameObject p_closestUnit = null;
        foreach(GameObject selection in possibleSelections){
            if(IsCloserToPosition(selection, closestObject, lastMousePosition)){
                closestObject = selection;
            }
            if(selection.GetComponent<PlayerUnit>()){
                if(p_closestUnit == null || IsCloserToPosition(selection,p_closestUnit,lastMousePosition)){
                    p_closestUnit = selection;
                }
            }
        }
        if(p_closestUnit != null){
            p_SelectedUnits = new List<GameObject>();
            p_SelectedUnits.Add(p_closestUnit);
            p_closestUnit.GetComponent<ISelectable>().Select();
            foreach(GameObject unit in p_SelectedUnits){
                unit.GetComponent<ISelectable>().Deselect();
            }
        }
        else{
            if(closestObject.GetComponent<ISelectable>() != null) closestObject.GetComponent<ISelectable>().Select();
        }

    }

    private bool IsCloserToPosition(GameObject a, GameObject b, Vector2 position){
        return Mathf.Abs(a.transform.position.sqrMagnitude - position.sqrMagnitude) < Mathf.Abs(b.transform.position.sqrMagnitude - position.sqrMagnitude);
    }

    private void IssueGroupCommand(List<GameObject> selectedUnits){

    }
}
