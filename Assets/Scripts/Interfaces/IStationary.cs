using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStationary: IOverlappable
{   

    // This variable indicates what this stationary object's render order should be relative to other stationary objects
    // It describes what the render order of this object should be when there are no overlapping objects, besides
    // stationary objects, if any.
    public int StationaryOverlapIndex{get; set;}

}
