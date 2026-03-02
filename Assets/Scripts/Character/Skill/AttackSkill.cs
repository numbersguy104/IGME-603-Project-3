using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackParam : SkillParam
{
    public float ATKRatio; // The damage this skill going to deal will be the ATK Ratio times the ATK of the skill user
    public List<Vector2> range;
    public List<StatusWithTurns> statusOnHit;
}

// An instance of a standard Attack Skill
public class AttackSkill: Skill
{
    public AttackSkill(SkillData skillData) : base(skillData)
    {
    }

    public override void Execute(Character_Combat instigator)
    {
        AttackParam attackParam = skillData.param as AttackParam;
        
        // TODO: Implement the damage
        // Grid.ApplyDamage(instigator, attackParam.range, attackParam.damageRatio * instigator.atk)
        // foreach status in attackParam.statusOnHit
        // Grid.ApplyStatus(instigator, attackParam.range, status)
    }
}
