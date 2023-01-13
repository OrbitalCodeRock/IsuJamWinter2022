using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOverlappable
{
    // Indicates which Overlap group an object belongs to (overlap groups are stored in the OverlapManager)
    public int OverlapGroupIndex{get; set;}

    public Collider2D SpriteOverlapCollider{get; set;}
}
