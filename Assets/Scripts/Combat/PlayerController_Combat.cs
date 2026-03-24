using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class PlayerController_Combat : SingletonBehavior<PlayerController_Combat>
{
    public List<PlayerCharacter_Combat> teamMembers;
    public PlayerCharacter_Combat currentCharacter;

    private bool isSelectingTile = false;
    public bool IsSelectingTile => isSelectingTile;
    private bool isAiming = false;
    public bool IsAiming => isAiming;
    public void Init(List<PlayerCharacter_Combat> teamMembers)
    {
        this.teamMembers = teamMembers;
        currentCharacter = teamMembers[0];
    }

    public void Attack()
    {
        EndTileSelecting();
        if (CombatManager.Instance.currentTurn != Team.Player)
            return;
        Debug.Log("Player Attack");
        GridManager.Instance.ApplyDamageToCells(currentCharacter, currentCharacter.TransformRangeToWorld(currentCharacter.attackRange.ToArray()), currentCharacter.ATK);
        currentCharacter.attacksAvailable--;
        CombatUI.Instance.UpdateCombatInfo();
        // EndPlayerTurn();
    }
    
    public void UseSkill(Skill skill)
    {
        if (CombatManager.Instance.currentTurn != Team.Player)
            return;
        EndTileSelecting();
        switch (skill)
        {
            case AttackSkill attack:
                if (attack.skillData.needAimingBeforeCast)
                {
                    StartAiming(attack);
                }
                else
                {
                    attack.Execute(currentCharacter);
                }
                // EndPlayerTurn();
                break;
            case Defense defense:
                defense.Execute(currentCharacter);
                break;
            case BuffSkill buff:
                buff.Execute(currentCharacter);
                break;
        }
    }

    public void UseItem(Item item)
    {
        EndTileSelecting();
        if (CombatManager.Instance.currentTurn != Team.Player)
            return;
        Debug.Log($"Player Use Item: {item.itemData.itemName}");
        item.UseInCombat(currentCharacter);
        // TODO: Remove Item from inventory
    }

    public void RequestMove()
    {
        if (CombatManager.Instance.currentTurn != Team.Player)
            return;
        if (isSelectingTile)
        {
            EndTileSelecting();
        }
        else
        {
            StartTileSelecting();
        }
    }

    public void StartTileSelecting()
    {
        StartCoroutine(nameof(TileSelecting));
    }

    private GridHighlight hoveredHighlight;
    IEnumerator TileSelecting()
    {
        Vector2Int[] highlightedTile = new[] { new Vector2Int(-1, -1) };
        isSelectingTile = true;
        hoveredHighlight = new GridHighlight(highlightedTile, HighlightType.Hovered);
        GridManager.Instance.AddHighlight(hoveredHighlight);
        while (!Input.GetMouseButtonDown(0) || highlightedTile[0].x < 0 || GridManager.Instance.GetAt(highlightedTile[0].x,  highlightedTile[0].y) != null)
        {
            highlightedTile[0] = GridManager.Instance.GetHoveredTile(true) ?? new Vector2Int(-1,-1);
            GridManager.Instance.RefreshHighlight();
            yield return null;
        }
        if(highlightedTile[0] != null)
            MoveTo(highlightedTile[0]);
        GridManager.Instance.RemoveHighlight(hoveredHighlight);
        isSelectingTile = false;
    }

    public void EndTileSelecting()
    {
        if (isSelectingTile)
        {
            GridManager.Instance.RemoveHighlight(hoveredHighlight);
            StopCoroutine(nameof(TileSelecting));
            isSelectingTile = false;
        }
    }
    
    public void StartAiming(object obj)
    {
        StartCoroutine(nameof(Aiming), obj);
    }
    private GridHighlight attackingHighlight;
    IEnumerator Aiming(object obj)
    {
        isAiming = true;
        Vector2Int[] highlightedTiles = null;
        switch (obj)
        {
            case AttackSkill attack:
                highlightedTiles = currentCharacter.TransformRangeToWorld(((AttackParam)attack.skillData.param).range.ToArray());
                attackingHighlight = new GridHighlight(highlightedTiles, HighlightType.Attacking);
                GridManager.Instance.AddHighlight(attackingHighlight);
                break;
        }
        while (!Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 target = ray.origin + ray.direction * (ray.origin.y / -ray.direction.y);
            Debug.Log(target);
            Vector3 direction = target - currentCharacter.entity.transform.position;
            direction.y = 0;
            for(int i = 1; i <= 4 && Vector3.Dot(direction.normalized,  currentCharacter.entity.transform.forward) < 0.707f; i++)
            {
                currentCharacter.entity.transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            currentCharacter.entity.Turn();
            switch (obj)
            {
                case AttackSkill attack:
                    Vector2Int[] newRange = currentCharacter.TransformRangeToWorld(((AttackParam)attack.skillData.param).range.ToArray());
                    for (int i = 0; i < highlightedTiles.Length; i++)
                        highlightedTiles[i] = newRange[i];
                    break;
            }
            
            GridManager.Instance.RefreshHighlight();
            yield return null;
        }
        
        switch (obj)
        {
            case Skill skill:
                skill.Execute(currentCharacter);
                break;
            case Item item:
                item.UseInCombat(currentCharacter);
                break;
        }
        
        GridManager.Instance.RemoveHighlight(attackingHighlight);
    }

    public void EndAiming()
    {
        if (isAiming)
        {
            isAiming = false;
            GridManager.Instance.RemoveHighlight(attackingHighlight);
            StopCoroutine(nameof(Aiming));
        }
    }

    public void MoveTo(Vector2Int tile)
    {
        if(GridManager.Instance.IsOccupied(tile.x, tile.y))
            return;
        Vector2Int currentPosition = GridManager.Instance.PosToGrid(currentCharacter.entity.transform.position);
        GridManager.Instance.Move(currentPosition.x, currentPosition.y, tile.x, tile.y, true);
        EndTileSelecting();
        currentCharacter.movesAvailable--;
        CombatUI.Instance.UpdateCombatInfo();
        // EndPlayerTurn();
    }
    
    public void SwitchCharacter()
    {
        // I'm assuming there are only two characters
        EndTileSelecting();
        EndAiming();
        currentCharacter = currentCharacter == teamMembers[0] ? teamMembers[1] : teamMembers[0];
        CombatUI.Instance.UpdateCombatInfo();
    }
    
    public void SwitchCharacter(int switchToCharacter) // 0 for Hugo, 1 for Tenet
    {
        EndTileSelecting();
        EndAiming();
        currentCharacter = teamMembers[switchToCharacter];
        CombatUI.Instance.UpdateCombatInfo();
    }

    public void EndPlayerTurn()
    {
        EndTileSelecting();
        EndAiming();
        
        CombatManager.Instance.EndTurn(Team.Player);
    }
}
