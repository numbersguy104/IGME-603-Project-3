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

    public virtual void Execute(Character_Combat instigator)
    {
    }

    public void NotifyTurnEnd()
    {
        cooldownRemained = Math.Max(cooldownRemained - 1, 0);
    }
}