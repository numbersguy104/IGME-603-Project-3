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

    public float level;
    public float exp;
    public float expToNextLevel;
    
    public Action OnLevelUp;
    
    #endregion

    public GameObject entityPrefab;

    public List<SkillData> skillSet;
    
    public Character(CharacterData data)
    {
        health = maxHealth = data.maxHP;
        skillPoint = maxSkillPoint = data.initialSkillPoint;
        ATK = data.ATK;
        DEF = data.DEF;
        maxMovementDistance = data.maxMovementDistance;
        skillSet = data.InitialSkillSet;
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
    
}