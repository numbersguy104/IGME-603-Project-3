using System;

public class Skill
{
    public SkillData skillData;
    public SkillType SkillType;
    public int cooldownRemained;
    public bool isReady => cooldownRemained == 0;
    public int maxCooldown;

    public Skill(SkillData skillData)
    {
        this.skillData = skillData;
        SkillType = skillData.skillType;
        cooldownRemained = 0;
        maxCooldown = skillData.cooldown;
    }

    /// <summary>
    /// Do something special when the skill is used. Only costing SP in this base class
    /// </summary>
    /// <param name="instigator">The skill user</param>
    public virtual void Execute(Character_Combat instigator)
    {
        if (instigator.CurrentSkillPoint < skillData.cost)
        {
            // TODO: No Enough SP
        }
        else
        {
            instigator.CostSP(skillData.cost);
        }

        cooldownRemained = maxCooldown;
    }

    public virtual void NotifyTurnEnd()
    {
        cooldownRemained = Math.Max(cooldownRemained - 1, 0);
    }
}