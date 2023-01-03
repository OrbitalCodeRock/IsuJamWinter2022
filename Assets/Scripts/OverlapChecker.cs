using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverlapChecker : MonoBehaviour
{ 
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
    private void Update(){

        List<Collider2D> spriteOverlaps = new List<Collider2D>(5);
        spriteOverlapCollider.OverlapCollider(OverlapManager.instance.spriteOverlapFilter, spriteOverlaps);
        if(spriteOverlaps.Count == 0){
             this.gameObject.GetComponent<SortingGroup>().sortingOrder = OverlapManager.instance.defaultUnitLayer;
             return;
        }
        int currentUnitSortingOrder = this.gameObject.GetComponent<SortingGroup>().sortingOrder;
        float currentUnitBaseHeight = this.gameObject.GetComponent<Collider2D>().bounds.min.y;
        for(int i = 0; i < spriteOverlaps.Count; i++){
            if(spriteOverlaps[i].transform.parent.GetComponent<SortingGroup>().sortingOrder <= currentUnitSortingOrder && 
            spriteOverlaps[i].GetComponent<Collider2D>().bounds.min.y < currentUnitBaseHeight){
                spriteOverlaps[i].transform.parent.GetComponent<SortingGroup>().sortingOrder = currentUnitSortingOrder + 1;
            }
        }
    }

    // private void OnTriggerStay2D(Collider2D col){

    //     Debug.Log("Test");
    //     if(col.gameObject.layer == GameManager.instance.mouseHitboxLayer){
    //         float otherUnitBaseHeight = col.transform.parent.GetComponent<BoxCollider2D>().bounds.center.y;
    //         float thisUnitBaseHeight = this.gameObject.GetComponent<BoxCollider2D>().bounds.center.y;
    //         if(otherUnitBaseHeight < thisUnitBaseHeight){
    //             col.transform.parent.GetComponent<SortingGroup>().sortingOrder = this.gameObject.GetComponent<SortingGroup>().sortingOrder + 1;
    //         }
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D col){
    //     List<Collider2D> spriteOverlaps = new List<Collider2D>(5);
    //     spriteOverlapCollider.OverlapCollider(OverlapManager.instance.spriteOverlapFilter, spriteOverlaps);
    //     foreach(Collider2D overlap in spriteOverlaps){  
    //         if(overlap != null) return;
    //     }
    //     this.gameObject.GetComponent<SortingGroup>().sortingOrder = OverlapManager.instance.defaultUnitLayer;
        
    // }
}
