using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverlapManager : MonoBehaviour
{
    public static OverlapManager instance;

    public ContactFilter2D spriteOverlapFilter;

    // Each overlap group contains the collider2Ds that enclose the visual sprite of an object.
    // This visual sprite is normally a child of the actual whole GameObject.
    public List<List<Collider2D>> overlapGroups;

    // A stack that allows us to quickly identify and access empty overlap groups.
    public Stack<int> emptyGroupIndicies;


    // The default sorting order of player units and enemy units, may also be the same as the default order for things like
    // trees and rocks.
    public int defaultUnitOrder;

    // public LayerMask spriteOverlapLayermask;

    private const int defaultGroupCapacity = 6;
    private const int defaultStationaryGroupCapacity = 3;

    void Awake(){
        if(instance != this && instance != null){
            Destroy(this);
        }
        else{
            instance = this;
        }
        overlapGroups = new List<List<Collider2D>>(15);
        // Maybe change this later to only add overlap groups as more are needed.
        // for(int i = 0; i < overlapGroups.Capacity; i++){
        //     overlapGroups.Add(new List<Collider2D>(defaultGroupCapacity));
        // }
        emptyGroupIndicies = new Stack<int>(overlapGroups.Capacity);
        for(int i = 0; i < overlapGroups.Capacity; i++){
            emptyGroupIndicies.Push(i);
        }

        stationaryOverlapGroups = new List<List<Collider2D>>(25);
        emptyStationaryGroupIndicies = new Stack<int>(stationaryOverlapGroups.Capacity);
        for(int i = 0; i < stationaryOverlapGroups.Capacity; i++){
            emptyStationaryGroupIndicies.Push(i);
        }
    }

    private void LateUpdate(){
        /* Loop through the list of overlapGroups
           for each list with more than 1 object
                Sort the list in descending order (based on minimum sprite bounding box height)
                Assign the first object in the list with the default render order for that type of object
                assign each successive object with a render order of 1 higher than the last
        */
        for(int grp = 0; grp < overlapGroups.Count; grp++){
            if(overlapGroups[grp].Count > 1){
                /* sort the overlap group, maybe I should consider figuring out if there is a faster sorting algorithim
                   that I could use */
                bool listSorted = false;
                while(!listSorted){
                    listSorted = true;
                    for(int col = 0; col < overlapGroups[grp].Count - 1; col++){
                        if(overlapGroups[grp][col].bounds.min.y < overlapGroups[grp][col + 1].bounds.min.y){
                            Collider2D temp = overlapGroups[grp][col];
                            overlapGroups[grp][col] = overlapGroups[grp][col + 1];
                            overlapGroups[grp][col + 1] = temp;
                            listSorted = false;
                        }
                    }
                }
                // Assign the proper sorting order to the overlap group
                for(int col = 0; col < overlapGroups[grp].Count; col++){
                    /* I should provide a faster way to access the sorting group of a given object (if possible), to allow for faster
                       performance */
                    overlapGroups[grp][col].transform.parent.GetComponent<SortingGroup>().sortingOrder = defaultUnitOrder + col;
                }
            }
        }
    }
    
    // Provide methods for adding elements to overlap groups.

    /* If this object is not in an overlap group and the other object is in an overlap group
       then this object should add itself to the other's overlap group.

       If this gameobject is not in an overlap group, and the other object is not in an overlap group,
       add both objects to the same overlap group. This may be acheivable just by adding the current object
       to an empty overlap group.
        
       If this gameobject is currently in an overlap group and the other object is not,
       do nothing, unless the other object is a stationary object, then add the other object
       to the current gameobject's overlap group.

            When adding stationary objects to an overlap group, we also have to add other stationary objects
            that overlap with that stationary object. Idea: check to see if both stationary objects are
            in the same overlap group, if not: add the stationary object and check that stationary object
            for other overlapping stationary objects

       If both gameobjects are in an overlap group, merge the groups

       So, we need a method for adding an object to another objects overlap group and we need a method for
       merging an overlap group into another overlap group. We also need a method for adding an object to an
       empty overlap group.
    */

    public void addToEmptyGroup(Collider2D objToAdd){
        int emptyGroupIndex;
        /* If the stack of group indicies is empty (happens when there are no more empty overlapGroups)
           then we need to create a new overlap group*/
        if(!emptyGroupIndicies.TryPop(out emptyGroupIndex)){
            overlapGroups.Add(new List<Collider2D>(defaultGroupCapacity));
            emptyGroupIndex = overlapGroups.Count - 1;
        }
        else{
            emptyGroupIndex = emptyGroupIndicies.Pop();
        }
        overlapGroups[emptyGroupIndex].Add(objToAdd);
        objToAdd.GetComponent<IOverlappable>().OverlapGroupIndex = emptyGroupIndex;
    }

    public void addToGroup(Collider2D objToAdd, int groupIndex){
        /* Trying to add the object to the group in a way such that the overlap group
           remains sorted. It might actually be better not to sort the group when adding the object, and to just
           add the element to the end (and change the sorting algorithim to work better for moving an element back to front).*/
        for(int i = 0; i < overlapGroups[groupIndex].Count; i++){
            if(overlapGroups[groupIndex][i].bounds.min.y <= objToAdd.bounds.min.y){
                overlapGroups[groupIndex].Insert(i, objToAdd);
                objToAdd.GetComponent<IOverlappable>().OverlapGroupIndex = groupIndex;
                return;
            }
        }
        // The code below should never really be reachable, since this method is not meant for adding to an empty group
        overlapGroups[groupIndex].Add(objToAdd);
        objToAdd.GetComponent<IOverlappable>().OverlapGroupIndex = groupIndex;
    }

    // Merges and empties elements from group 2 into group 1.
    public void mergeGroups(int groupIndex1, int groupIndex2){
        List<Collider2D> mergeList = new List<Collider2D>(10);

        int limitingGroup = groupIndex1;
        int largerGroup = groupIndex2;
        if(overlapGroups[groupIndex2].Count < overlapGroups[groupIndex1].Count){
            limitingGroup = groupIndex2;
            largerGroup = groupIndex1;
        }

        /* An attempt to somewhat quickly merge the two lists so that they come out sorted,
           this attempt will succeed if the two lists are already sorted in descending order */
        for(int i = 0; i < overlapGroups[limitingGroup].Count; i++){
            if(overlapGroups[limitingGroup][i].bounds.min.y >= overlapGroups[largerGroup][i].bounds.min.y){
                mergeList.Add(overlapGroups[limitingGroup][i]);
                mergeList.Add(overlapGroups[largerGroup][i]);
            }
            else{
                mergeList.Add(overlapGroups[largerGroup][i]);
                mergeList.Add(overlapGroups[limitingGroup][i]);
            }
            
            // The newley merged list will replace the overlapGroup with index "groupIndex1"
            overlapGroups[limitingGroup][i].GetComponent<IOverlappable>().OverlapGroupIndex = groupIndex1;
            overlapGroups[largerGroup][i].GetComponent<IOverlappable>().OverlapGroupIndex = groupIndex1;
        }

        for(int i = overlapGroups[limitingGroup].Count; i < overlapGroups[largerGroup].Count; i++){
            mergeList.Add(overlapGroups[largerGroup][i]);
            overlapGroups[largerGroup][i].GetComponent<IOverlappable>().OverlapGroupIndex = groupIndex1;
        }

        overlapGroups[groupIndex1] = mergeList;
        overlapGroups[groupIndex2] = new List<Collider2D>(defaultGroupCapacity);
        emptyGroupIndicies.Push(groupIndex2);
        
    }

    // Provide methods for removing objects from overlap groups

    /* Idea for removing elements. Look at the element to be removed, do a collider cast to see what that objects that element
       overlaps with, and more casts to see what objects those elements overlap with. If those elements are not the objects
       in the overlap group, move those elements into a new overlap group, try to keep them sorted for best performance.
    */

    public List<List<Collider2D>> stationaryOverlapGroups;
    public Stack<int> emptyStationaryGroupIndicies;

    public void addToEmptyStationaryGroup(Collider2D objToAdd){
        int emptyGroupIndex;
        /* If the stack of group indicies is empty (happens when there are no more empty overlapGroups)
           then we need to create a new overlap group*/
        if(!emptyStationaryGroupIndicies.TryPop(out emptyGroupIndex)){
            stationaryOverlapGroups.Add(new List<Collider2D>(defaultGroupCapacity));
            emptyGroupIndex = stationaryOverlapGroups.Count - 1;
        }
        else{
            emptyGroupIndex = emptyStationaryGroupIndicies.Pop();
        }
        stationaryOverlapGroups[emptyGroupIndex].Add(objToAdd);
        objToAdd.GetComponent<IStationary>().StationaryGroupIndex = emptyGroupIndex;
    }
    public void addToStationaryGroup(Collider2D objToAdd, int groupIndex){
        /* Trying to add the object to the group in a way such that the overlap group
           remains sorted. */
        for(int i = 0; i < stationaryOverlapGroups[groupIndex].Count; i++){
            if(stationaryOverlapGroups[groupIndex][i].bounds.min.y <= objToAdd.bounds.min.y){
                stationaryOverlapGroups[groupIndex].Insert(i, objToAdd);
                objToAdd.GetComponent<IStationary>().StationaryGroupIndex = groupIndex;
                return;
            }
        }
        // The code below should never really be reachable, since this method is not meant for adding to an empty group
        stationaryOverlapGroups[groupIndex].Add(objToAdd);
        objToAdd.GetComponent<IStationary>().StationaryGroupIndex = groupIndex;
    }

    public void copyFromStationaryGroup(int groupIndex, int stationaryGroupIndex){
        List<Collider2D> mergeList = new List<Collider2D>(10);

        int limitingCount = overlapGroups[groupIndex].Count;
        int largerCount = stationaryOverlapGroups[stationaryGroupIndex].Count;
        if(largerCount < limitingCount){
            int temp = limitingCount;
            limitingCount = largerCount;
            largerCount = temp;
        }

        /* An attempt to somewhat quickly merge the two lists so that they come out sorted,
           this attempt will succeed if the two lists are already sorted in descending order */
        for(int i = 0; i < limitingCount; i++){
            if(stationaryOverlapGroups[stationaryGroupIndex][i].bounds.min.y >= overlapGroups[groupIndex][i].bounds.min.y){
                mergeList.Add(stationaryOverlapGroups[stationaryGroupIndex][i]);
                mergeList.Add(overlapGroups[groupIndex][i]);
            }
            else{
                mergeList.Add(overlapGroups[groupIndex][i]);
                mergeList.Add(stationaryOverlapGroups[stationaryGroupIndex][i]);
            }
            
            // The newley merged list will replace the overlapGroup with index "groupIndex1"
            overlapGroups[groupIndex][i].GetComponent<IOverlappable>().OverlapGroupIndex = groupIndex;
            stationaryOverlapGroups[stationaryGroupIndex][i].GetComponent<IOverlappable>().OverlapGroupIndex = groupIndex;
        }

        if(largerCount == stationaryOverlapGroups[stationaryGroupIndex].Count){
            for(int i = limitingCount; i < largerCount; i++){
                mergeList.Add(stationaryOverlapGroups[stationaryGroupIndex][i]);
                stationaryOverlapGroups[stationaryGroupIndex][i].GetComponent<IOverlappable>().OverlapGroupIndex = groupIndex;
            }
        }
        else{
            for(int i = limitingCount; i < largerCount; i++){
                mergeList.Add(overlapGroups[groupIndex][i]);
                overlapGroups[groupIndex][i].GetComponent<IOverlappable>().OverlapGroupIndex = groupIndex;
            }
        }

        overlapGroups[groupIndex] = mergeList;
    }


}
