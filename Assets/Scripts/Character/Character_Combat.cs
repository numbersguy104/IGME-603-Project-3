using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Character_Combat
{
    #region Attributes
    protected float health;
    protected float maxHealth;
    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;
    protected float mana;
    protected float maxMana;
    public float CurrentMana => mana;
    public float MaxMana => maxMana;

    public float ATK;
    public float Def;
    
    public List<Vector2> attackRange;
    #endregion
    
    public List<Skill> skills;
    public List<Status> statuses;

    public Action OnTurnEnd;
    public Action<Character_Combat> OnCharacterDeath;
    public Action OnTakeDamage;
    
    public Character_Combat(Character character)
    {
        health = character.CurrentHealth;
        maxHealth = character.MaxHealth;
        mana = character.CurrentMana;
        maxMana = character.MaxMana;
        ATK = character.ATK;
        //Get attackRange from character (according to the weapon?);
        
        skills =  new List<Skill>();
        foreach (var skillData in character.skillSet)
        {
            Skill skill = new Skill(skillData);
            skills.Add(skill);
        }
        
        statuses = new List<Status>();
    }
    
    public virtual void OnNotifiedTurnEnd()
    {
        foreach (Skill skill in skills)
        {
            skill.NotifyTurnEnd();
        }
        
        OnTurnEnd?.Invoke();
    }
    
    public virtual void Attack()
    {
        
    }

    public virtual void UseSkill()
    {
        
    }
    
    public void Healed(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        // VFX?
    }

    public void TakeDamage(float amount)
    {
        // TODO: How does the Defense affect the damage?
        
        health = Mathf.Max(health - amount, 0);
        OnTakeDamage?.Invoke();
        // TODO: VFX?
        
        if (health == 0)
        {
            OnCharacterDeath?.Invoke(this);
        }
        
    }

    public void AddStatus(StatusData statusData, int turns)
    {
        Status existingStatus = statuses.FirstOrDefault(s => s.statusData.statusName == statusData.statusName);
        if (existingStatus != null)
        {
            existingStatus.Extend(turns);
        }
        else
        {
            Status newStatus = StatusFactory.GetStatus(statusData.statusName, this, turns);
            statuses.Add(newStatus);
        }
    }

}