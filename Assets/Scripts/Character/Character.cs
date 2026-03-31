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

    public int level = 0;
    public float exp;
    public float expToNextLevel = 10;
    public float expGrowth = 5;
    
    #endregion

    public GameObject entityPrefab;

    public int maxSkillSlots;
    public List<SkillData> skillEquipped;
    public List<SkillData> skillLearned;
    
    public Character(CharacterData data)
    {
        health = maxHealth = data.maxHP;
        skillPoint = maxSkillPoint = data.initialSkillPoint;
        ATK = data.ATK;
        DEF = data.DEF;
        maxMovementDistance = data.maxMovementDistance;
        skillLearned = data.InitialSkillSet;
        skillEquipped = new List<SkillData>();
        maxSkillSlots = data.initialSkillSlot;
        for(int i = 0; i < Math.Min(maxSkillSlots, skillLearned.Count); i++)
        {
            skillEquipped.Add(skillLearned[i]);
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

}