using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffParam : SkillParam
{
    public bool applyOnAllies;
    public List<Vector2> range;
    public List<StatusWithTurns> statusList;
}

public class BuffSkill: Skill
{
    public BuffSkill(SkillData skillData) : base(skillData)
    {
    }

    public override void Execute(Character_Combat instigator)
    {
        BuffParam buffParam = skillData.param as BuffParam;
        // TODO: Implement applying status
        // foreach status,turns in buffParam.statusList
        // if(buffParam.applyOnAllies)
        //  CombatManager.ApplyStatus(instigator is Player, status, turns)
        // else
        //  Grid.ApplyStatus(instigator, buffParam.range, status, turns)
    }
}
