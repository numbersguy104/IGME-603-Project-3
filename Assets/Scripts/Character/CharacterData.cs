using System.Collections.Generic;
using UnityEngine;

public class CharacterData : ScriptableObject
{
    public float maxHP;
    public float maxMP;
    public float ATK;
    public float DEF;
    public float maxMovementDistance;

    public GameObject entity;
    // public float armor;
    public List<SkillData> InitialSkillSet;
}