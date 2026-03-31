using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerCharacter : Character
{
    protected PlayerData playerData;
    public SkillData normalAttack;
    public Action OnLevelUp;
    
    public PlayerCharacter(PlayerData data) : base(data)
    {
        playerData = data;
        normalAttack = data.normalAttack;
    }
    
    public void GainExperience(float amount)
    {
        exp += amount;

        if (expToNextLevel > 0)
        {
            while (exp >= expToNextLevel)
            {
                exp -= expToNextLevel;
                level++;
                expToNextLevel += expGrowth;
                List<SkillData> skillsToBeUnlocked = playerData.skillUnlockedAtLevel.Where(x => x.level == level).Select(x => x.skill).ToList();
                foreach (var skill in skillsToBeUnlocked)
                    LearnNewSkill(skill);
                OnLevelUp?.Invoke();
            }
        }
    }
    
    public void AddSkillSlot(int increment)
    {
        maxSkillSlots += increment;
    }

    public void LearnNewSkill(SkillData skill)
    {
        skillLearned.Add(skill);
    }

}