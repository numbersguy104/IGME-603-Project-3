using System;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    #region Attributes
    public float health;
    public float CurrentHealth => health;
    protected float maxHealth;
    public float MaxHealth => maxHealth;
    
    protected float mana;
    public float CurrentMana => mana;
    protected float maxMana;
    public float MaxMana => maxMana;

    public float ATK;

    public float level;
    public float exp;
    public float expToNextLevel;
    
    public Action OnLevelUp;
    
    #endregion
    
    public List<SkillData> skillSet;
    
    public Character(CharacterData data)
    {
        health = maxHealth = data.maxHP;
        mana = maxMana = data.maxMP;
        ATK = data.ATK;
        skillSet = data.InitialSkillSet;
    }

    public void Healed(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    public void UpdateStateFromCombat(Character_Combat character)
    {
        health = character.CurrentHealth;
        mana = character.CurrentMana;
    }

    public void GainExperience(float amount)
    {
        exp += amount;
        while (exp > expToNextLevel)
        {
            exp -= expToNextLevel;
            level++;
            OnLevelUp?.Invoke();
        }
    }
    
}