using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridRange : Range
{
    public List<Vector2Int> relativeCoordinates;

    public override Vector2Int[] GetAllTileCovered(Character_Combat owner)
    {
        return owner.TransformRangeToWorld(relativeCoordinates.ToArray()); 
    }
}