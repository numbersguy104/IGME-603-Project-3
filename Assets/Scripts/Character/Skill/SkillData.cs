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
    public int cooldown; // How many turns it takes before the character can use this skill again

    [SerializeReference] public SkillParam param;
}
