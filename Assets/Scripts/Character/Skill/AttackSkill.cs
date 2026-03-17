using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackParam : SkillParam
{
    [Tooltip("The damage this skill going to deal will be the ATK Ratio times the ATK of the skill user")]
    public float ATKRatio;
    [Tooltip("The relative coordinates of the cells to be affected when the skill is used.\n (x,y) means x cells to the right and y cell forward.")]
    public List<Vector2Int> range;
    [Tooltip("List of the status to be added to the character hit by this skill")]
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
        base.Execute(instigator);
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

        if (instigator is PlayerCharacter_Combat playerCharacter)
        {
            playerCharacter.attacksAvailable--;
            CombatUI.Instance.UpdateCombatInfo();
        }
    }
}
