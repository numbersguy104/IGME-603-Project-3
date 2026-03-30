using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileDistanceRange : Range
{
    public int distance;

    public override Vector2Int[] GetAllTileCovered(Character_Combat owner)
    {
        return GridManager.Instance.GetTilesWithinRange(GridManager.Instance.PosToGrid(owner.entity.transform.position), distance, true);
    }
}