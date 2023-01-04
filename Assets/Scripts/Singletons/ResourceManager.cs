using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public int wood;
    public int stone;
    public int food;
    
    [SerializeField]
    private Item[] gameItems;

    // An array of Items in which the index of each Item corresponds with their item id.
    [HideInInspector]
    public Item[] itemDatabase;

    // An array of integers that represent the quantities of items within itemDatabase.
    private int[] itemQuantities;

    void Awake(){
        if(this != instance && instance != null){
            Destroy(this);
        }
        else{
            instance = this;
        }
        itemQuantities = new int[gameItems.Length]; 
        // This can break if there exists an item id that is greater than or equal to gameItems.length
        itemDatabase = new Item[gameItems.Length];
        foreach(Item item in gameItems){
            itemDatabase[item.id] = item;
        }
    }
}
