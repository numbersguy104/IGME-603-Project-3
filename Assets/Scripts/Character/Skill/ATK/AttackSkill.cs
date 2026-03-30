using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AttackParam : SkillParam
{
    [Tooltip("The damage this skill going to deal will be the ATK Ratio times the ATK of the skill user")]
    public float ATKRatio;
    
    [Serializable]
    public struct EffectByChances
    {
        public StatusWithTurns statusWithTurns;
        public float chance;
    }
    [Tooltip("List of the status to be added to the character hit by this skill")]
    public List<EffectByChances> statusOnHit;
}

// An instance of a standard Attack Skill
public class AttackSkill: Skill
{
    public RangeType rangeType;
    
    public AttackSkill(SkillData skillData) : base(skillData)
    {
    }

    public override void Execute(Character_Combat instigator)
    {
        base.Execute(instigator);
        AttackParam attackParam = skillData.param as AttackParam;

        float dmg = attackParam.ATKRatio * instigator.ATK;
        Status_PoweredUp status_powerup = instigator.GetStatus(typeof(Status_PoweredUp)) as Status_PoweredUp;
        if (status_powerup != null)
        {
            dmg *= status_powerup.atk_multiplier;
        }
        
        void ApplyStatus(Character_Combat source, Character_Combat target, float dmg)
        {
            foreach (var effectWithChances in attackParam.statusOnHit)
                if(Random.Range(0f, 1f) < effectWithChances.chance)
                    target.AddStatus(effectWithChances.statusWithTurns.status, effectWithChances.statusWithTurns.turns);
        }

        GridManager.Instance.ApplyDamageToTiles(instigator,
            skillData.range.GetAllTileCovered(instigator),
            dmg,
            ApplyStatus
            );

        if (status_powerup != null)
        {
            instigator.RemoveAllStatus(typeof(Status_PoweredUp));
        }
        
        if (instigator is PlayerCharacter_Combat playerCharacter)
        {
            playerCharacter.attacksAvailable--;
            CombatUI.Instance.UpdateCombatInfo();
        }
        
    }
    
    public override void Execute(Character_Combat instigator, Character_Combat target)
    {
        base.Execute(instigator);
        AttackParam attackParam = skillData.param as AttackParam;

        float dmg = attackParam.ATKRatio * instigator.ATK;
        Status_PoweredUp status_powerup = instigator.GetStatus(typeof(Status_PoweredUp)) as Status_PoweredUp;
        if (status_powerup != null)
        {
            dmg *= status_powerup.atk_multiplier;
        }
        
        if (target.team != instigator.team)
        {
            target.TakeDamage(dmg);
            foreach (var effectWithChances in attackParam.statusOnHit)
                if(Random.Range(0f, 1f) < effectWithChances.chance)
                    target.AddStatus(effectWithChances.statusWithTurns.status, effectWithChances.statusWithTurns.turns);
        }

        if (status_powerup != null)
        {
            instigator.RemoveAllStatus(typeof(Status_PoweredUp));
        }
        
        if (instigator is PlayerCharacter_Combat playerCharacter)
        {
            playerCharacter.attacksAvailable--;
            CombatUI.Instance.UpdateCombatInfo();
        }
        
    }
}
