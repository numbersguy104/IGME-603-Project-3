using System.Collections.Generic;
using UnityEngine;

public class CharacterData : ScriptableObject
{
    public float maxHP;
    public float maxMP;
    public float ATK;
    // public float armor;
    public List<SkillData> InitialSkillSet;
}