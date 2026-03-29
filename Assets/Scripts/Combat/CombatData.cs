using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatData", menuName = "ScriptableObject/Combat/CombatData")]
public class CombatData : ScriptableObject
{
    public Vector2Int gridSize;
    public List<EnemyData> enemies;
    public List<Obstacle> obstacles;
}

[Serializable]
public struct Obstacle
{
    public Vector2Int position;
    public GameObject prefab;
}
