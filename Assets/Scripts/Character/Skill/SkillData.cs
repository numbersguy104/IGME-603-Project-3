using System;
using UnityEngine;
public enum SkillType
{
    Attack,
    Defense,
    Buff
}

[Serializable] public class SkillParam{}

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObject/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public Sprite skillIcon;
    public SkillType skillType;
    public float cost; // Mana cost
    public int cooldown; // How many turns it takes before the character can use this skill again
    public bool needAimingBeforeCast; // If set to false, the skill will be executed according to the current direction

    [SerializeReference] public SkillParam param;
}
