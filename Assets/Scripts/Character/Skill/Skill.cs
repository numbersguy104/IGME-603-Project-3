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
    /// Do something special when the skill is used. Only costing mana in this base class
    /// </summary>
    /// <param name="instigator">The skill user</param>
    public virtual void Execute(Character_Combat instigator)
    {
        if (instigator.CurrentMana < skillData.cost)
        {
            // TODO: No Enough Mana
        }
        else
        {
            instigator.CostMana(skillData.cost);
        }
    }

    public void NotifyTurnEnd()
    {
        cooldownRemained = Math.Max(cooldownRemained - 1, 0);
    }
}