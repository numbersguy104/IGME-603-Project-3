using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Utility;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class PlayerController_Combat : SingletonBehavior<PlayerController_Combat>
{
    public List<PlayerCharacter_Combat> teamMembers;
    public PlayerCharacter_Combat currentCharacter;
    public Characters currentCharacterName => currentCharacter == null || currentCharacter == teamMembers[0] ? Characters.HUGO : Characters.TENET;

    private bool isSelectingTile = false;
    public bool IsSelectingTile => isSelectingTile;
    
    private bool isSelectingPosition = false;
    public bool IsSelectingPosition => isSelectingPosition;

    private bool isSelectingTarget;
    public bool IsSelectingTarget => isSelectingTarget;
    
    private bool isAiming = false;
    public bool IsAiming => isAiming;
    public void Init(List<PlayerCharacter_Combat> teamMembers)
    {
        this.teamMembers = teamMembers;
        currentCharacter = teamMembers[0];
    }

    public void Attack()
    {
        if (CombatManager.Instance.currentTurn != Team.Player)
            return;
        EndAllSelecting();
        if(currentCharacter == teamMembers[0])
            UseSkill(currentCharacter.normalAttack);
        else
            StartSelecting(SelectionType.Target);
        // EndPlayerTurn();
    }
    
    public void UseSkill(Skill skill)
    {
        if (CombatManager.Instance.currentTurn != Team.Player)
            return;
        EndAllSelecting();
        switch (skill)
        {
            case AttackSkill attack:
                if (attack.skillData.needAiming)
                {
                    StartSelecting(SelectionType.Orientation, attack);
                }
                else
                {
                    attack.Execute(currentCharacter);
                }
                break;
            // case Defense defense:
            //     defense.Execute(currentCharacter);
            //     break;
            case BuffSkill buff:
                if (buff.skillData.needAiming)
                    StartSelecting(SelectionType.Orientation, buff);
                else
                    buff.Execute(currentCharacter);
                break;
        }
    }

    public void UseItem(Item item)
    {
        EndAllSelecting();
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
        EndAllSelecting();
        if (currentCharacter == teamMembers[0])
        {
            if (isSelectingTile)
            {
                EndAllSelecting();
            }
            else
            {
                StartSelecting(SelectionType.Tile);
            }
        }
        else
        {
            if (isSelectingPosition)
            {
                EndAllSelecting();
            }
            else
            {
                StartSelecting(SelectionType.Position);
            }
        }
    }

    public int GetDistanceTo(Vector2Int targetTile)
    {
        Vector2Int currentTile = GridManager.Instance.PosToGrid(currentCharacter.entity.transform.position);
        return Math.Abs(currentTile.x - targetTile.x) +  Math.Abs(currentTile.y - targetTile.y);;
    }
    
    public float GetDistanceTo(Vector3 targetPosition)
    {
        Vector3 displacement = currentCharacter.entity.transform.position - targetPosition;
        displacement.y = 0;
        return displacement.magnitude;
    }

    private enum SelectionType
    {
        Tile,
        Position,
        Orientation,
        Target
    }
    private void StartSelecting(SelectionType selectionType, object obj = null)
    {
        switch (selectionType)
        {
            case SelectionType.Tile:
                StartCoroutine(nameof(TileSelecting));
                break;
            case SelectionType.Position:
                StartCoroutine(nameof(PositionSelecting));
                break;
            case SelectionType.Orientation:
                StartCoroutine(nameof(Aiming), obj);
                break;
            case SelectionType.Target:
                StartCoroutine(nameof(TargetSelecting));
                break;
        }
    }
    public void EndAllSelecting()
    {
        if (isSelectingTile)
        {
            GridManager.Instance.RemoveHighlight(hoveredHighlight);
            GridManager.Instance.RemoveHighlight(rangeHighlight);
            StopCoroutine(nameof(TileSelecting));
            isSelectingTile = false;
        }
        if (isSelectingPosition)
        {
            StopCoroutine(nameof(PositionSelecting));
            currentCharacter.entity.HideRange();
            isSelectingPosition = false;
        }
        if (isSelectingTarget)
        {
            StopCoroutine(nameof(TargetSelecting));
            currentCharacter.entity.HideRange();
            isSelectingTarget = false;
        }
        if (isAiming)
        {
            GridManager.Instance.RemoveHighlight(highlight);
            StopCoroutine(nameof(Aiming));
            isAiming = false;
        }
    }

    private GridHighlight hoveredHighlight;
    private GridHighlight rangeHighlight;
    IEnumerator TileSelecting()
    {
        Vector2Int[] selectedTile = new[] { new Vector2Int(-1, -1) };
        isSelectingTile = true;
        hoveredHighlight = new GridHighlight(selectedTile, HighlightType.Hovered);
        GridManager.Instance.AddHighlight(hoveredHighlight);
        Vector2Int[] tilesInrange = GridManager.Instance.GetTilesWithinRange(GridManager.Instance.PosToGrid(currentCharacter.entity.transform.position), (int)currentCharacter.maxMovementDistance);
        rangeHighlight = new GridHighlight(tilesInrange, HighlightType.InMovingRange);
        GridManager.Instance.AddHighlight(rangeHighlight);
        while (true)
        {
            selectedTile[0] = GridManager.Instance.GetHoveredTile(true) ?? new Vector2Int(-1,-1);
            GridManager.Instance.RefreshHighlight();
            yield return null;
            if (Input.GetMouseButtonDown(0) && selectedTile[0].x != -1 && GridManager.Instance.GetAt(selectedTile[0].x,  selectedTile[0].y) == null && GetDistanceTo(selectedTile[0]) <= currentCharacter.maxMovementDistance)
                break;
            if(Input.GetMouseButtonDown(1))
                EndAllSelecting();
        }
        if(selectedTile[0].x != -1)
            MoveTo(selectedTile[0]);
        GridManager.Instance.RemoveHighlight(hoveredHighlight);
        GridManager.Instance.RemoveHighlight(rangeHighlight);
        isSelectingTile = false;
    }

    IEnumerator PositionSelecting()
    {
        isSelectingPosition = true;
        Vector3? targetPosition = null;
        currentCharacter.entity.DisplayRange(currentCharacter.movesAvailable * currentCharacter.maxMovementDistance, RoundRangeHighlight.Move);
        while (true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("CombatGround")))
                targetPosition = hit.point;
            else
                targetPosition = null;
            yield return null;
            if (Input.GetMouseButtonDown(0) && targetPosition != null)
                break;
            if(Input.GetMouseButtonDown(1))
                EndAllSelecting();
        }

        if (targetPosition != null)
        {
            float distance = Mathf.Min(Vector3.Distance(targetPosition.Value, currentCharacter.entity.transform.position),
                currentCharacter.movesAvailable * currentCharacter.maxMovementDistance);
            Vector3 restrictedPosition = Vector3.MoveTowards(currentCharacter.entity.transform.position, targetPosition.Value, distance);
            MoveTo(restrictedPosition);
        }
        currentCharacter.entity.HideRange();
        isSelectingPosition = false;
    }
    
    IEnumerator TargetSelecting()
    {
        isSelectingTarget = true;
        GameObject target = null;
        CombatEntity targetEntity = null;
        Vector2Int targetPosition;
        currentCharacter.entity.DisplayRange((currentCharacter.normalAttack.skillData.range as CircleRange).radius, RoundRangeHighlight.Attack);
        while (true)
        {
            targetPosition = GridManager.Instance.GetHoveredTile(true) ?? new Vector2Int(-1,-1);
            target = GridManager.Instance.GetAt(targetPosition.x, targetPosition.y);
            if (target != null && target.TryGetComponent(out CombatEntity entity) && entity.character.team != Team.Player)
            {
                targetEntity = entity;
                targetEntity.SetSpriteHighlight();
            }
            yield return null;
            if (Input.GetMouseButtonDown(0) && targetPosition.x >= 0 && target !=null && target.GetComponent<CombatEntity>().character.team != Team.Player)
                break;
            if(Input.GetMouseButtonDown(1))
                EndAllSelecting();
        }
        if(targetEntity != null)
            currentCharacter.normalAttack.Execute(currentCharacter, targetEntity.character);
        
        currentCharacter.entity.HideRange();
        isSelectingPosition = false;

        // if(DataTracker.Instance != null)
        //     DataTracker.Instance.AddSkillUse(currentCharacter.normalAttack);
    }
    
    private GridHighlight highlight;
    IEnumerator Aiming(object obj)
    {
        isAiming = true;
        Vector2Int[] highlightedTiles = new Vector2Int[GridManager.Instance.Size.x * GridManager.Instance.Size.y];
        switch (obj)
        {
            case AttackSkill attack:
                Vector2Int[] tilesToAttack = attack.skillData.range.GetAllTileCovered(currentCharacter);
                for (int i = 0; i < tilesToAttack.Length; i++)
                    highlightedTiles[i] = tilesToAttack[i];
                highlight = new GridHighlight(highlightedTiles, HighlightType.InAttackingRange);
                GridManager.Instance.AddHighlight(highlight);
                break;
            case BuffSkill buff:
                Vector2Int[] tilesToBuff = buff.skillData.range.GetAllTileCovered(currentCharacter);
                for (int i = 0; i < tilesToBuff.Length; i++)
                    highlightedTiles[i] = tilesToBuff[i];
                highlight = new GridHighlight(highlightedTiles, HighlightType.InBuffRange);
                GridManager.Instance.AddHighlight(highlight);
                break;
        }
        while (true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 target;
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("CombatGround")))
                target = hit.point;
            else
                target = ray.origin + ray.direction * (ray.origin.y / -ray.direction.y);
            Vector3 direction = target - currentCharacter.entity.transform.position;
            direction.y = 0;
            if(currentCharacter == teamMembers[0])
                for(int i = 1; i <= 4 && Vector3.Dot(direction.normalized,  currentCharacter.entity.transform.forward) < 0.707f; i++)
                {
                    currentCharacter.entity.transform.rotation *= Quaternion.Euler(0, 90, 0);
                }
            else
            {
                currentCharacter.entity.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);   
            }
            currentCharacter.entity.Turn();
            switch (obj)
            {
                case AttackSkill attack:
                    Vector2Int[] newRangeToAttack = attack.skillData.range.GetAllTileCovered(currentCharacter);
                    for (int i = 0; i < newRangeToAttack.Length; i++)
                        highlightedTiles[i] = newRangeToAttack[i];

                    for (int i = newRangeToAttack.Length; i < highlightedTiles.Length; i++)
                        highlightedTiles[i] = new (-1, -1);
                    break;
                case BuffSkill buff:
                    Vector2Int[] newRangeToBuff = buff.skillData.range.GetAllTileCovered(currentCharacter);
                    for (int i = 0; i < newRangeToBuff.Length; i++)
                        highlightedTiles[i] = newRangeToBuff[i];

                    for (int i = newRangeToBuff.Length; i < highlightedTiles.Length; i++)
                        highlightedTiles[i] = new (-1, -1);
                    break;
            }
            
            GridManager.Instance.RefreshHighlight();
            yield return null;

            if (Input.GetMouseButtonDown(0) && hit.transform != null)
                break;
            if(Input.GetMouseButtonDown(1))
                EndAllSelecting();
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
        
        GridManager.Instance.RemoveHighlight(highlight);
    }

    public void MoveTo(Vector2Int tile)
    {
        if(GridManager.Instance.IsOccupied(tile.x, tile.y))
            return;
        Vector2Int currentPosition = GridManager.Instance.PosToGrid(currentCharacter.entity.transform.position);
        GridManager.Instance.Move(currentPosition.x, currentPosition.y, tile.x, tile.y, true);
        EndAllSelecting();
        currentCharacter.movesAvailable -= 1;
        CombatUI.Instance.UpdateCombatInfo();
        // EndPlayerTurn();
    }
    
    public void MoveTo(Vector3 targetPosition)
    {
        targetPosition.y = currentCharacter.entity.transform.position.y;
        Vector3 displacement = targetPosition - currentCharacter.entity.transform.position;
        Vector2Int targetTile = GridManager.Instance.PosToGrid(targetPosition);
        if(GridManager.Instance.IsOccupied(targetTile.x, targetTile.y))
            return;
        Vector2Int currentPosition = GridManager.Instance.PosToGrid(currentCharacter.entity.transform.position);
        GridManager.Instance.Move(currentPosition.x, currentPosition.y, targetTile.x, targetTile.y, false);
        currentCharacter.entity.MoveTo(targetPosition);
        EndAllSelecting();
        CombatUI.Instance.UpdateCombatInfo();
        // EndPlayerTurn();
    }
    
    public void SwitchCharacter()
    {
        // Assuming there are only two characters
        EndAllSelecting();
        currentCharacter = currentCharacter == teamMembers[0] ? teamMembers[1] : teamMembers[0];
        CombatUI.Instance.UpdateCombatInfo();
    }
    
    public void SwitchCharacter(int switchToCharacter) // 0 for Hugo, 1 for Tenet
    {
        EndAllSelecting();
        currentCharacter = teamMembers[switchToCharacter];
        CombatUI.Instance.UpdateCombatInfo();
    }

    public void EndPlayerTurn()
    {
        EndAllSelecting();
        CombatManager.Instance.EndTurn(Team.Player);
    }

    private void Update()
    {
        if (CombatManager.Instance.currentTurn == Team.Player && DataTracker.Instance != null)
        {
            if (currentCharacterName == Characters.HUGO)
            {
                DataTracker.Instance.hugoTime += Time.deltaTime;
            }
            else if (currentCharacterName == Characters.TENET)
            {
                DataTracker.Instance.tenetTime += Time.deltaTime;
            }
        }
    }
}
