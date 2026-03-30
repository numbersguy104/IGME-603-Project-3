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
    protected float skillPoint;
    protected float maxSkillPoint;
    public float CurrentSkillPoint => skillPoint;
    public float MaxSkillPoint => maxSkillPoint;

    public float ATK;
    public float DEF;
    public float maxMovementDistance;
    
    public List<Vector2Int> attackRange;
    #endregion

    public Team team;
    
    public CombatEntity entity;

    public List<Skill> skills;
    public List<Status> statuses;
    public float ATK_Multiplier;

    public Action<Character_Combat> OnTurnStart;
    public Action<Character_Combat> OnTurnEnd;
    public Action<Character_Combat> OnCharacterDeath;
    public Action OnTakeDamage;
    public Action OnSPChanged;
    public Action OnStatusUpdated;
    
    /// <summary>
    /// Create Combat Character Instance from a Character
    /// </summary>
    /// <param name="character">The target character</param>
    public Character_Combat(Character character)
    {
        health = character.CurrentHealth;
        maxHealth = character.MaxHealth;
        skillPoint = character.CurrentSkillPoint;
        maxSkillPoint = character.MaxSkillPoint;
        ATK = character.ATK;
        DEF = character.DEF;
        maxMovementDistance = character.maxMovementDistance;
        
        //TODO: Get attackRange from character (according to the weapon?);
        attackRange = new List<Vector2Int> { new(0, 1) }; // For week-1 Test Only
        
        skills = new List<Skill>();
        foreach (var skillData in character.skillEquipped)
        {
            Skill skill = SkillFactory.GetSkill(skillData);
            if(skill != null)
                skills.Add(skill);
        }
        
        statuses = new List<Status>();
    }

    /// <summary>
    /// Invoked when the character's turn starts.
    /// </summary>
    public virtual void OnNotifiedTurnStart()
    {
        OnTurnStart?.Invoke(this);
    }
    
    /// <summary>
    /// Invoked when the character's turn ends. Trigger all events that happen at the turn ending phase. After that, update skill CD and status turn counter.
    /// </summary>
    public virtual void OnNotifiedTurnEnd()
    {
        OnTurnEnd?.Invoke(this);
        
        foreach (Skill skill in skills)
        {
            skill.NotifyTurnEnd();
        }
        for(int i = 0; i < statuses.Count;)
        {
            Status status = statuses[i];
            status.NotifyTurnEnd();
            if (status.TurnsRemained != 0)
                i++;
        }
        
        OnStatusUpdated?.Invoke();
    }

    /// <summary>
    /// Simply cost SP. Technically can also be used to refill SP.
    /// </summary>
    /// <param name="cost">The amount of SP to be cost. Negative if to refill mana</param>
    public virtual void CostSP(float cost)
    {
        skillPoint -= cost;
        OnSPChanged?.Invoke();
    }
    
    /// <summary>
    /// Simply Heal the character. Technically can also be used to do damage (Not recommended). Will trigger OnTakeDamage event;
    /// </summary>
    /// <param name="amount">The amount of health to be healed. cannot exceed maximum HP</param>
    public virtual void Healed(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        // VFX?
        
        OnTakeDamage?.Invoke(); // Warning: Currently Used to Trigger UI Update. Maybe at risk
    }

    /// <summary>
    /// Damage the character. Will trigger OnTakeDamage event and OnCharacterDeath if HP <= 0
    /// </summary>
    /// <param name="amount"> The amount of damage to be dealt. </param>
    public virtual void TakeDamage(float amount)
    {
        float dmg = amount / (1 + DEF / 10f); 
        
        health = Mathf.Max(health - dmg, 0);
        OnTakeDamage?.Invoke();

        entity.EntityGetAttacked();
        
        if (health <= 0)
        {
            Die();
        }
        
    }

    public virtual void Die()
    {
        OnCharacterDeath?.Invoke(this);
    }

    /// <summary>
    /// Add a status to the character alone with the turns it will last for
    /// </summary>
    /// <param name="statusData"> The status to be applied </param>
    /// <param name="turns">The number of turns it will last for</param>
    public virtual void AddStatus(StatusData statusData, int turns)
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

    public Status GetStatus(Type statusType)
    {
        foreach (var status in statuses)
        {
            if (statusType.IsAssignableFrom(status.GetType()))
                return status;
        }
    
        return null;
    }
    
    public void RemoveAllStatus(Type statusType)
    {
        for(int i = 0; i < statuses.Count;)
        {
            Status status = statuses[i];
            if (statusType.IsAssignableFrom(status.GetType()))
            {
                statuses.Remove(status);
                continue;
            }
            i++;
        }
        OnStatusUpdated?.Invoke();
    }

    /// <summary>
    /// Use this to transform local coordinates of skill/attack range into the absolute coordinates in the grid
    /// </summary>
    /// <param name="coors"> The coordinates to be transformed </param>
    /// <returns>The transformed coordinates. </returns>
    public Vector2Int[] TransformRangeToWorld(Vector2Int[] coors)
    {
        Vector2Int[] result = new Vector2Int[coors.Length];
        int index = 0;
        foreach (var coordinate in coors)
        {
            result[index] = GridManager.Instance.PosToGrid(entity.transform.position + coordinate.x * entity.transform.right +  coordinate.y * entity.transform.forward);
            index++;
        }

        return result;
    }
    
    /// <summary>
    /// Another overload method for range that's not on the grid
    /// </summary>
    public Vector2Int[] TransformRangeToWorld(Vector2[] coors)
    {
        Vector2Int[] result = new Vector2Int[coors.Length];
        int index = 0;
        foreach (var coordinate in coors)
        {
            result[index] = GridManager.Instance.PosToGrid(entity.transform.position + coordinate.x * entity.transform.right +  coordinate.y * entity.transform.forward);
            index++;
        }

        return result;
    }
}