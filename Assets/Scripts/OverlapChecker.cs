using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverlapChecker : MonoBehaviour, IOverlappable
{ 
    public int OverlapGroupIndex{get; set;} = -1;

    public Collider2D SpriteOverlapCollider{get; set;}

    /* There are two seperate colliders on most objects, one that overlaps with the entire sprite and one that
        overlaps with the base of the sprite (the "feet" or bottom of the sprite that you would reasonably bump into
        if it were 3D */

    // We need to properly render if any part of the sprite overlaps with any part of any other sprite.

    /* However, we need to order the sprite based on where the "feet" of the sprites are located, because this dictates
       whether or not one sprite is in front of the other */


    private void OnTriggerEnter2D(Collider2D other){
        /* If the other object is in an overlap group and this object is not in an overlap group,
       then this object should add itself to the other's overlap group.

       If both gameobjects are in an overlap group, merge the two overlap groups into one.

       If the other object is not in an overlap group and this gameobject is currently in an overlap group,
       do nothing, unless the other object is a stationary object, then add the other object
       to the current gameobject's overlap group.

            When adding stationary objects to an overlap group, we also have to add other stationary objects
            that overlap with that stationary object.

       If both objects are not in an overlap group, add both objects to the same overlap group. 
       This may be acheivable just by adding the current object to an empty overlap group.
       (the other object should theoretically add itself)

       */
        IOverlappable otherOverlapInfo = other.GetComponent<IOverlappable>();
        if(otherOverlapInfo != null){
            if(otherOverlapInfo.OverlapGroupIndex != -1){
               if(OverlapGroupIndex != -1){
                  /*
                    If the other object is in an overlap group and this object is not in an overlap group,
                    then this object should add itself to the other's overlap group.
                  */
                  OverlapManager.instance.mergeGroups(OverlapGroupIndex, otherOverlapInfo.OverlapGroupIndex); 
               }
               else{
                  // If both gameobjects are in an overlap group, merge the two overlap groups into one.
                  OverlapManager.instance.addToGroup(SpriteOverlapCollider, otherOverlapInfo.OverlapGroupIndex);
               }
            }
            else{
               if(OverlapGroupIndex == -1){
                  OverlapManager.instance.addToEmptyGroup(SpriteOverlapCollider);
               }

               /* If the other object is stationary, we need to add that object (and any other stationary objects touching that object)
                  to the same overlap group */
               IStationary otherStationaryInfo = other.GetComponent<IStationary>();

               if(otherStationaryInfo != null){
                  /* Get a list of stationary objects overlapping with the other object (as well as itself),
                     add all of them to the overlap group this object belongs to */
                  /* Idea: 
                     1. Add the other object to this objects overlap group
                     2. Call GetOverlappingStationaries() on other which returns a list of overlapping stationaries (and not the calling object)
                     3. Loop through the returned list:
                        For each item:
                           1. If the object is not already in an overlap group: add the object to this objects overlap group
                           2. Call GetOverlappingStationaries() on the object:
                              If the list is empty, do nothing
                              If the list is not empty:
                                 for each item: 
                                 If the object is not already in an overlap group: add the object to this objects overlap group
                                 If the object is not in this objects overlap group: merge the two overlap groups
                                    Maybe we could have some sort of quick or simple merge for this
                  */
                  /* Better Idea:
                     Within the overlap Manager, store stationary objects in special stationary sorting groups.
                     These stationary sorting groups only have to be updated when a new object is added or removed from them.

                     If the other object is in a stationary sorting group, add the whole group to this object's sorting group
                     If the other object is not in a stationary sorting group, just add it to this object's overlap group

                  */
                  if(otherStationaryInfo.StationaryGroupIndex != -1){
                     OverlapManager.instance.copyFromStationaryGroup(OverlapGroupIndex, otherStationaryInfo.StationaryGroupIndex);
                  }
                  else{
                     OverlapManager.instance.addToGroup(otherOverlapInfo.SpriteOverlapCollider, OverlapGroupIndex);
                  }
                  
               }
            }
        }
     }

     private void OnTriggerExit2D(){
      
     }

     /* The old method below worked ok, but there is an overflow risk since this method only increases
       the render order of other objects when necessary (as opposed to decreasing it). 
       It also seems redundant/inefficent (but simplier to setup) */
    // private void Update(){

    //     List<Collider2D> spriteOverlaps = new List<Collider2D>(5);
    //     spriteOverlapCollider.OverlapCollider(OverlapManager.instance.spriteOverlapFilter, spriteOverlaps);
    //     if(spriteOverlaps.Count == 0){
    //          this.gameObject.GetComponent<SortingGroup>().sortingOrder = OverlapManager.instance.defaultUnitOrder;
    //          return;
    //     }
    //     SortingGroup currentUnitSortingGroup = this.gameObject.GetComponent<SortingGroup>();
    //     float currentUnitBaseHeight = this.gameObject.GetComponent<Collider2D>().bounds.min.y;
    //     for(int i = 0; i < spriteOverlaps.Count; i++){
    //         SortingGroup otherUnitSortingGroup = spriteOverlaps[i].transform.parent.GetComponent<SortingGroup>();
    //         if(otherUnitSortingGroup.sortingOrder <= currentUnitSortingGroup.sortingOrder && 
    //         spriteOverlaps[i].GetComponent<Collider2D>().bounds.min.y < currentUnitBaseHeight)
    //         {
    //             spriteOverlaps[i].transform.parent.GetComponent<SortingGroup>().sortingOrder = currentUnitSortingGroup.sortingOrder + 1;
    //         }
    //     }
    // }
}
