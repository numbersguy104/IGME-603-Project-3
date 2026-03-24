using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CircleRange : Range
{
    public float radius;

    public override Vector2Int[] GetAllTileCovered(Character_Combat owner)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        Transform from = owner.entity.transform;
        for(int x = 0; x < GridManager.Instance.Size.x;x++)
            for (int y = 0; y < GridManager.Instance.Size.y; y++)
            {
                Vector3 tilePos = GridManager.Instance.GridToPos(new Vector2Int(x,y));
                tilePos.y = from.position.y;
                // Check the 4 corners of a tile
                if(CheckInCircle(from, tilePos + new Vector3(0.5f, 0, 0.5f)) ||
                   CheckInCircle(from, tilePos + new Vector3(0.5f, 0, -0.5f)) ||
                   CheckInCircle(from, tilePos + new Vector3(-0.5f, 0, 0.5f)) ||
                   CheckInCircle(from, tilePos + new Vector3(-0.5f, 0, -0.5f)))
                    res.Add(new Vector2Int(x,y));
            }

        return res.ToArray();
    }

    private bool CheckInCircle(Transform from, Vector3 point)
    {
        return Vector3.Distance(point, from.position) < radius;
    }
}