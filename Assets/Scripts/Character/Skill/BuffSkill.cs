using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffParam : SkillParam
{
    [Serializable]
    public struct EffectByChances
    {
        public StatusWithTurns statusWithTurns;
        public float chance;
    }
    public List<EffectByChances> statusForAllies;
    public List<EffectByChances> statusForEnemies;
}

// An instance of a standard Attack Skill
public class BuffSkill: Skill
{
    public RangeType rangeType;
    
    public BuffSkill(SkillData skillData) : base(skillData)
    {
    }

    public override void Execute(Character_Combat instigator)
    {
        base.Execute(instigator);
        BuffParam buffParam = skillData.param as BuffParam;

        foreach (var effectWithChances in buffParam.statusForAllies)
            GridManager.Instance.ApplyEffectToTiles(instigator,
                skillData.range.GetAllTileCovered(instigator),
                effectWithChances,
                true
            );
        
        foreach (var effectWithChances in buffParam.statusForEnemies)
            GridManager.Instance.ApplyEffectToTiles(instigator,
                skillData.range.GetAllTileCovered(instigator),
                effectWithChances,
                false
            );

        CombatUI.Instance.UpdateCombatInfo();
        
        if(DataTracker.Instance != null)
            DataTracker.Instance.AddSkillUse(this);
    }
}
