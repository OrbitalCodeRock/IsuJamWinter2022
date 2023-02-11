using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverlapChecker : MonoBehaviour, IOverlappable
{ 
    public int OverlapGroupIndex{get; set;} = -1;
   
   // a bit wierd, but I want to be able to set this within the inspector
    public Collider2D spriteOverlapCollider;
    public Collider2D SpriteOverlapCollider{get; set;}

    public void Awake(){
      SpriteOverlapCollider = spriteOverlapCollider;
    }

    /* There are two seperate colliders on most objects, one that overlaps with the entire sprite and one that
        overlaps with the base of the sprite (the "feet" or bottom of the sprite that you would reasonably bump into
        if it were 3D */

    // We need to properly render if any part of the sprite overlaps with any part of any other sprite.

    /* However, we need to order the sprite based on where the "feet" of the sprites are located, because this dictates
       whether or not one sprite is in front of the other */

    private void OnCollisionEnter2D(Collision2D collision){
         Debug.Log("OnColEnter");
         // The if statement below needs to be changed, does not function
         // if(collision.collider.gameObject.layer != GameManager.instance.mouseHitboxLayer.value){
         //    return;
         // }

        /* 
        If both gameobjects are in an overlap group, merge the two overlap groups into one.
        (unless they are already in the same overlap group)
        
         If the other object is in an overlap group and this object is not in an overlap group,
       then this object should add itself to the other's overlap group.

       If the other object is not in an overlap group and this gameobject is currently in an overlap group,
       do nothing, unless the other object is a stationary object, then add the other object
       to the current gameobject's overlap group.

            When adding stationary objects to an overlap group, we also have to add other stationary objects
            that overlap with that stationary object.

       If both objects are not in an overlap group, add both objects to the same overlap group. 
       This may be acheivable just by adding the current object to an empty overlap group.
       (the other object should theoretically add itself)

       */
        IOverlappable otherOverlapInfo = collision.collider.GetComponentInParent<IOverlappable>();
        if(otherOverlapInfo != null){
            Debug.Log("Other object's group index: " + otherOverlapInfo.OverlapGroupIndex);
            if(otherOverlapInfo.OverlapGroupIndex != -1){
               if(OverlapGroupIndex != otherOverlapInfo.OverlapGroupIndex && OverlapGroupIndex != -1){
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
                  Debug.Log("Success?");
                  OverlapManager.instance.addToEmptyGroup(SpriteOverlapCollider);
                  Debug.Log("This object's new overlap index: " + OverlapGroupIndex);
               }

               /* If the other object is stationary, we need to add that object (and any other stationary objects touching that object)
                  to the same overlap group */
               // I am considering reworking stationary objects so that they will add themselves to overlap groups,
               // In that case, I'll need to change how this works.
               IStationary otherStationaryInfo = collision.collider.GetComponentInParent<IStationary>();

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

      /* 
       Idea for removing elements: Look at the element to be removed, do a collider cast to see what that objects that element
       overlaps with, if none of those objects are within this gameobject's overlap group, remove this
       gameobject from the overlap group.

       When removing the last element from the group, push the index of the group onto the emptyGroupIndicies stack

       If the other object (the one that the current object is exiting from onTriggerExit) is a stationary object,
       if the object is in a stationary group, have all objects in that group do a collider cast, if none of those
       stationary objects are overlapping non-stationary objects within the overlap group, remove all the stationary
       objects in the stationary group from the overlap group

            This will be easier if we loop through all the overlapping objects and return as soon as
            a non-stationary object within the overlap group is found.

      I need to provide a method for splitting up overlap groups into smaller groups, this is not implemented yet
    */

     private void OnCollisionExit2D(Collision2D collision){
         Debug.Log("OnColExit");
         // The if statement below needs to be changed, does not function
         // if(collision.collider.gameObject.layer != GameManager.instance.mouseHitboxLayer.value){
         //    return;
         // }
         // We shouldn't have to check to leave an overlap group unless we are detaching from an object in the same overlap group
         if(collision.collider.GetComponentInParent<IOverlappable>() != null && collision.collider.GetComponentInParent<IOverlappable>().OverlapGroupIndex == OverlapGroupIndex){
            // Find out if the current object should remove itself from an overlap group
            Collider2D[] spriteOverlaps = new Collider2D[30];
            SpriteOverlapCollider.OverlapCollider(OverlapManager.instance.spriteOverlapFilter, spriteOverlaps);
            if(ShouldLeaveGroup(spriteOverlaps, OverlapGroupIndex)){
               OverlapManager.instance.removeFromGroup(SpriteOverlapCollider, OverlapGroupIndex);
            }


            // If the other object is a stationary object, we may have to remove it from the overlap group as well.
            // I am considering reworking stationary objects so that they will remove themselves from overlap groups,
            // In that case, I'll need to change how this works.
            IStationary otherStationaryInfo = collision.collider.GetComponent<IStationary>();
            if(otherStationaryInfo != null){
               // If the stationary object is not part of a stationary group, we do a similar check for removal
               if(otherStationaryInfo.StationaryGroupIndex == -1){
                  spriteOverlaps = new Collider2D[30];
                  otherStationaryInfo.SpriteOverlapCollider.OverlapCollider(OverlapManager.instance.spriteOverlapFilter, spriteOverlaps);
                  if(ShouldLeaveGroup(spriteOverlaps, otherStationaryInfo.OverlapGroupIndex)){
                     OverlapManager.instance.removeFromGroup(otherStationaryInfo.SpriteOverlapCollider, OverlapGroupIndex);
                  }
               }
               else{
                  List<Collider2D> stationaryGroup = OverlapManager.instance.stationaryOverlapGroups[otherStationaryInfo.StationaryGroupIndex];
                  for(int i = 0; i < stationaryGroup.Count; i++){
                     spriteOverlaps = new Collider2D[30];
                     stationaryGroup[i].OverlapCollider(OverlapManager.instance.spriteOverlapFilter, spriteOverlaps);
                     if(!ShouldLeaveGroup(spriteOverlaps, otherStationaryInfo.OverlapGroupIndex)){
                        return;
                     }
                  }
                  // remove the entire stationary group from the overlap group
                  int groupIndex = otherStationaryInfo.OverlapGroupIndex;
                  for(int i = 0; i < stationaryGroup.Count; i++){
                     OverlapManager.instance.removeFromGroup(stationaryGroup[i], groupIndex);
                  }
               }
            }
         }
     }

      private bool ShouldLeaveGroup(Collider2D[] spriteOverlaps, int groupIndex){
         for(int i = 0; i < spriteOverlaps.Length; i++){
            if(spriteOverlaps[i] == null) return true;
            IOverlappable overlapInfo = spriteOverlaps[i].GetComponentInParent<IOverlappable>();
            if(overlapInfo != null && overlapInfo.OverlapGroupIndex == OverlapGroupIndex){
               return false;
            }
         }
         return true;
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
