using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverlapChecker : MonoBehaviour, IOverlappable
{ 
    public int OverlapGroupIndex{get; set;} = -1;

    public GameObject spriteOverlapObject;

    private BoxCollider2D spriteOverlapCollider;
    private void Awake(){
        spriteOverlapCollider = spriteOverlapObject.GetComponent<BoxCollider2D>();
    }

    /* There are two seperate colliders on most objects, one that overlaps with the entire sprite and one that
        overlaps with the base of the sprite (the "feet" or bottom of the sprite that you would reasonably bump into
        if it were 3D */

    // We need to properly render if any part of the sprite overlaps with any part of any other sprite.

    /* However, we need to order the sprite based on where the "feet" of the sprites are located, because this dictates
       whether or not one sprite is in front of the other */

    
    /* The method below seems to work well for now, but there is an overflow risk since this method only increases
       the render order of other objects when necessary (as opposed to decreasing it). Theres probably a way to get around
       this */
    // Maybe allowing the current unit to decrease its own render order as long as it stays at or above the default order would work.
    private void Update(){

        List<Collider2D> spriteOverlaps = new List<Collider2D>(5);
        spriteOverlapCollider.OverlapCollider(OverlapManager.instance.spriteOverlapFilter, spriteOverlaps);
        if(spriteOverlaps.Count == 0){
             this.gameObject.GetComponent<SortingGroup>().sortingOrder = OverlapManager.instance.defaultUnitOrder;
             return;
        }
        SortingGroup currentUnitSortingGroup = this.gameObject.GetComponent<SortingGroup>();
        float currentUnitBaseHeight = this.gameObject.GetComponent<Collider2D>().bounds.min.y;
        for(int i = 0; i < spriteOverlaps.Count; i++){
            SortingGroup otherUnitSortingGroup = spriteOverlaps[i].transform.parent.GetComponent<SortingGroup>();
            if(otherUnitSortingGroup.sortingOrder <= currentUnitSortingGroup.sortingOrder && 
            spriteOverlaps[i].GetComponent<Collider2D>().bounds.min.y < currentUnitBaseHeight)
            {
                spriteOverlaps[i].transform.parent.GetComponent<SortingGroup>().sortingOrder = currentUnitSortingGroup.sortingOrder + 1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        /* If the other gameobject is currently in an overlap group and this object is not
           do nothing, unless the other object is a stationary object, in which case this
           object should add itself to the other overlap group */

        /* if this gameobject is not in an overlap group, and the other object is not in an overlap group,
           add both objects to the same overlap group
        */
        
        /* If this gameobject is currently in an overlap group and the other object is not,
           add the other gameobject to the current gameobject's overlap group.   
        */

        /* If both gameobjects are in an overlap group, let the gameobject with the highest render order 
           merge the overlap groups into one. This provides a higher chance of creating an already sorted
           overlap group (sorted by ascending order of minimum sprite bounding box height)
        */

        if(OverlapGroupIndex == -1){
           
        }
    }
}
