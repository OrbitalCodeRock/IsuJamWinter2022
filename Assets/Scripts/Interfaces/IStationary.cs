using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStationary: IOverlappable
{   

    // This variable indicates what stationary sorting group this object belongs to.
    public int StationaryGroupIndex{get; set;}


}
