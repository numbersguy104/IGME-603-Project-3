using System;
using System.Collections.Generic;
using UnityEngine;

public enum Characters
{
    HUGO,
    TENET
}

public class Character
{
    #region Attributes
    public float health;
    public float CurrentHealth => health;
    protected float maxHealth;
    public float MaxHealth => maxHealth;
    
    protected float skillPoint;
    public float CurrentSkillPoint => skillPoint;
    protected float maxSkillPoint;
    public float MaxSkillPoint => maxSkillPoint;

    public float ATK;
    public float DEF;
    public float maxMovementDistance;

    public float level = 0;
    public float exp;
    public float expToNextLevel = 30;
    
    public Action OnLevelUp;
    
    #endregion

    public GameObject entityPrefab;

    public int maxSkillSlots = 2;
    public List<SkillData> skillSet;
    public List<SkillData> skillLearned;
    
    public Character(CharacterData data)
    {
        health = maxHealth = data.maxHP;
        skillPoint = maxSkillPoint = data.initialSkillPoint;
        ATK = data.ATK;
        DEF = data.DEF;
        maxMovementDistance = data.maxMovementDistance;
        skillLearned = data.InitialSkillSet;
        skillSet = new List<SkillData>();
        foreach (var skillData in skillLearned)
        {
            skillSet.Add(skillData);
        }
        entityPrefab = data.entity;
    }

    public void Healed(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    public void UpdateStateFromCombat(Character_Combat character)
    {
        health = character.CurrentHealth;
        skillPoint = character.CurrentSkillPoint;
    }

    public void GainExperience(float amount)
    {
        exp += amount;

        if (expToNextLevel > 0)
        {
            while (exp > expToNextLevel)
            {
                exp -= expToNextLevel;
                level++;
                OnLevelUp?.Invoke();
            }
        }
    }

    public void AddSkillSlot(int increment)
    {
        maxSkillSlots += increment;
    }
    
}