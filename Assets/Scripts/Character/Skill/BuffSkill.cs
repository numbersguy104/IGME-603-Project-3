using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffParam : SkillParam
{
    [Tooltip("If this buff is applied to all allies at any position")]
    public bool applyOnAllies;
    [Tooltip("The relative coordinates of the cells to be affected. Only used when applyOnAllies is false")]
    public List<Vector2> range;
    [Tooltip("The status to be applied along with the turns it will last for")]
    public List<StatusWithTurns> statusList;
}

public class BuffSkill: Skill
{
    public BuffSkill(SkillData skillData) : base(skillData)
    {
    }

    public override void Execute(Character_Combat instigator)
    {
        base.Execute(instigator);
        BuffParam buffParam = skillData.param as BuffParam;
        // TODO: Implement applying status
        // foreach status,turns in buffParam.statusList
        // if(buffParam.applyOnAllies)
        //  CombatManager.ApplyStatus(instigator is Player, status, turns)
        // else
        //  Grid.ApplyStatus(instigator, buffParam.range, status, turns)
    }
}
