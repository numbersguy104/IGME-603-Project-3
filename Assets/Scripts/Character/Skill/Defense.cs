using System;

[Serializable]
public class DefenseParam : SkillParam
{
    public float resistance;
}

public class Defense: Skill
{
    public Defense(SkillData skillData) : base(skillData)
    {
    }

    public override void Execute(Character_Combat instigator)
    {
        base.Execute(instigator);
        // TODO: Increase this instigator's damage resistance in this turn
    }
}
