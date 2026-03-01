using UnityEngine;

/// <summary>
/// CharacterStatsSO is a ScriptableObject that stores character stats such as move speed and maximum HP.
/// </summary>
[CreateAssetMenu(menuName = "Game/Character Stats")]
public class CharacterStatsSO : ScriptableObject
{
    /// Movement speed
    public float moveSpeed = 5f;

    /// Maximum health
    public float maxHP = 100f;
}
