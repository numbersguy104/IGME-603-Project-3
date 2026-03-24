using System.Collections.Generic;

public class PlayerCharacter : Character
{
    public SkillData normalAttack;
    public PlayerCharacter(PlayerData data) : base(data)
    {
        normalAttack = data.normalAttack;
    }

}