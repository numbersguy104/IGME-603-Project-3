using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Team
{
    Player,
    Enemy
}
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
    
    public List<Vector2Int> attackRange;
    #endregion

    public Team team;
    
    public CombatEntity entity;
    
    public List<Skill> skills;
    public List<Status> statuses;

    public Action OnTurnEnd;
    public Action<Character_Combat> OnCharacterDeath;
    public Action OnTakeDamage;
    public Action OnStatusUpdated;
    
    public Character_Combat(Character character)
    {
        health = character.CurrentHealth;
        maxHealth = character.MaxHealth;
        mana = character.CurrentMana;
        maxMana = character.MaxMana;
        ATK = character.ATK;
        
        //Get attackRange from character (according to the weapon?);
        attackRange = new List<Vector2Int> { new(0, 1) };
        
        skills =  new List<Skill>();
        foreach (var skillData in character.skillSet)
        {
            Skill skill = null;
            switch (skillData.skillType)
            {
                case SkillType.Attack: 
                    skill = new AttackSkill(skillData);
                    break;
                case SkillType.Defense:
                    skill = new Defense(skillData);
                    break;
                case SkillType.Buff:
                    skill = new BuffSkill(skillData);
                    break;
            }
            if(skill != null)
                skills.Add(skill);
        }
        
        statuses = new List<Status>();
    }
    
    public virtual void OnNotifiedTurnEnd()
    {
        OnTurnEnd?.Invoke();
        
        foreach (Skill skill in skills)
        {
            skill.NotifyTurnEnd();
        }
        for(int i = 0; i < statuses.Count;)
        {
            Status status = statuses[i];
            status.NotifyTurnEnd();
            if (status.TurnsRemained > 0)
                i++;
        }
        
        OnStatusUpdated?.Invoke();
    }
    
    public void Healed(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        // VFX?
        
        OnTakeDamage?.Invoke(); // Warning: Used For Trigger UI Update. Maybe at risk
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
            Status newStatus = StatusFactory.GetStatus(statusData, this, turns);
            statuses.Add(newStatus);
            newStatus.OnStatusCleared += () =>
            {
                statuses.Remove(newStatus);
                OnStatusUpdated?.Invoke();
            };
        }
        
        OnStatusUpdated?.Invoke();
    }

    public Vector2Int[] TransformRangeToWorld(Vector2Int[] coors)
    {
        Vector2Int[] result = new Vector2Int[coors.Length];
        Vector2Int currentCoor = GridManager.Instance.PosToGrid(entity.transform.position);
        int index = 0;
        foreach (var coordinate in coors)
        {
            result[index] = coordinate + currentCoor;
            index++;
        }

        return result;
    }
}