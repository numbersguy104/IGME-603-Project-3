using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackParam : SkillParam
{
    public float ATKRatio; // The damage this skill going to deal will be the ATK Ratio times the ATK of the skill user
    public List<Vector2Int> range;
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

        void ApplyStatus(Character_Combat source, Character_Combat target, float dmg)
        {
            foreach (var status in attackParam.statusOnHit)
                target.AddStatus(status.status, status.turns);
        }

        GridManager.Instance.ApplyDamageToCells(instigator, 
            instigator.TransformRangeToWorld(attackParam.range.ToArray()),
            attackParam.ATKRatio * instigator.ATK,
            ApplyStatus
            );
    }
}
