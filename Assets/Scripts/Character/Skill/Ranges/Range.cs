using System;
using UnityEngine;

[Serializable]
public abstract class Range
{
    // public abstract bool CheckInRange(Transform from, Vector3 target);
    
    public abstract Vector2Int[] GetAllTileCovered(Character_Combat owner);
}

public enum RangeType
{
    Grid,
    Circle,
    Sector,
    TileDistance
}