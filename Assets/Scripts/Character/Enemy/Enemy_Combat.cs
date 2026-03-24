using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Combat: Character_Combat
{
    struct PositionData
    {
        public Vector2Int tile;
        public int rotation;
    }
    
    public Enemy_Combat(Character data) : base(data)
    {
        team = Team.Enemy;
        CombatManager.Instance.OnEnemyTurnEnd?.AddListener(OnNotifiedTurnEnd);
    }

    ~Enemy_Combat()
    {
        CombatManager.Instance?.OnEnemyTurnEnd?.RemoveListener(OnNotifiedTurnEnd);
    }

    public override void OnNotifiedTurnEnd()
    {
        base.OnNotifiedTurnEnd();
    }
    
    public override void Die()
    {
        
    }

    //From a list of tiles that can be moved to (validTiles),
    //get the "best" tile from which to use an ability with the given range (targetRange)
    //that can target the given entities (targets).
    //Will also check rotations of the range if needsAiming is true.
    //Returns null if the targetRange cannot reach any targets from anywhere in validTiles.
    private PositionData? GetTargetingTile(List<Vector2Int> validTiles, List<Vector2Int> targetRange_, List<Character_Combat> targets, bool needsAiming)
    {
        //Store the number of targets that can be hit from a given tile position and direction.
        Dictionary<PositionData, int> targetCounts = new Dictionary<PositionData, int>();

        List<Vector2Int> targetRange = targetRange_;
        int highestTargetCount = 0;

        for (int rotation = 0; (rotation < 360 && needsAiming) || rotation < 90; rotation += 90)
        {
            //Checking the attack from every valid tile would be slow.
            //Instead, check the range backwards from each of the targets.
            foreach (Character_Combat target in targets)
            {
                Vector2Int targetPos = GridManager.Instance.PosToGrid(target.entity.transform.position);
                foreach (Vector2Int rangeOffset in targetRange)
                {
                    Vector2Int attackingTile = targetPos - rangeOffset;
                    if (validTiles.Contains(attackingTile))
                    {
                        PositionData attackingPosition = new PositionData { tile = attackingTile, rotation = rotation };
                        if (!targetCounts.ContainsKey(attackingPosition))
                        {
                            targetCounts.Add(attackingPosition, 0);
                        }
                        targetCounts[attackingPosition] += 1;
                        highestTargetCount = Math.Max(highestTargetCount, targetCounts[attackingPosition]);
                    }
                }
            }

            if (needsAiming)
            {
                List<Vector2Int> newTargetRange = new List<Vector2Int>();
                foreach (Vector2Int tile in targetRange)
                {
                    newTargetRange.Add(new Vector2Int(tile.y, -tile.x)); //rotate 90 degrees
                }
                targetRange = newTargetRange;
            }
        }

        //Make a list of the tiles that reach the most targets at once
        List<PositionData> highestPositions = new List<PositionData>();
        foreach (KeyValuePair<PositionData, int> pair in targetCounts) {
            if (pair.Value == highestTargetCount)
            {
                highestPositions.Add(pair.Key);
            }
        }

        if (highestPositions.Count > 0)
        {
            PositionData finalPos = SelectFarthestTile(highestPositions);
            return finalPos;
        }

        return null;
    }

    //Out of the PositionData in the list, returns the farthest one from the player characters.
    //Uses the following algorithm:
    //Select the tile that has the highest minimum of distances from the player characters.
    //Tiebreak by selecting the tile that has the highest sum of distances from the player characters.
    //If still tied, choose randomly.
    private PositionData SelectFarthestTile(List<PositionData> positions)
    {
        int ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        List<Vector2Int> playerPositions = new List<Vector2Int>();
        foreach (PlayerCharacter_Combat character in CombatManager.Instance.playerCharacters_Combat)
        {
            Vector2Int gridPos = GridManager.Instance.PosToGrid(character.entity.transform.position);
            playerPositions.Add(gridPos);
        }

        List<PositionData> highestMinPositions = new List<PositionData>();
        int highestMin = 0;
        foreach (PositionData pos in positions)
        {
            int minDistance = int.MaxValue;
            foreach (Vector2Int playerPos in playerPositions)
            {
                minDistance = Math.Min(minDistance, ManhattanDistance(pos.tile, playerPos));
            }

            if (minDistance > highestMin)
            {
                highestMinPositions.Clear();
                highestMin = minDistance;
            }
            if (minDistance >= highestMin)
            {
                highestMinPositions.Add(pos);
            }
        }

        List<PositionData> highestSumPositions = new List<PositionData>();
        int highestSum = 0;
        foreach (PositionData pos in highestMinPositions)
        {
            int distanceSum = 0;
            foreach (Vector2Int playerPos in playerPositions)
            {
                distanceSum += ManhattanDistance(pos.tile, playerPos);
            }

            if (distanceSum > highestSum)
            {
                highestSumPositions.Clear();
                highestSum = distanceSum;
            }
            if (distanceSum >= highestSum)
            {
                highestSumPositions.Add(pos);
            }
        }

        int randIndex = UnityEngine.Random.Range(0, highestSumPositions.Count);
        return highestSumPositions[randIndex];
    }

    public IEnumerator Act()
    {
        //Pause before each enemy action to give the player time to process
        yield return new WaitForSeconds(1.0f);

        Vector2Int currentPosition = GridManager.Instance.PosToGrid(entity.transform.position);

        //Get all tiles the enemy could move to this turn
        List<Vector2Int> movableTiles = new List<Vector2Int>();
        for (int x = 0; x < GridManager.Instance.Size.x; x++)
        {
            for (int y = 0; y < GridManager.Instance.Size.y; y++)
            {
                // TODO: Check movement cost

                if (GridManager.Instance.GetAt(x, y) == null)
                {
                    movableTiles.Add(new Vector2Int(x, y));
                }
            }
        }
        //Add the enemy's current position (representing no movement)
        //since otherwise it would be considered as occupied
        movableTiles.Add(GridManager.Instance.PosToGrid(entity.transform.position));

        Skill chosenSkill = null;
        bool shouldBasicAttack = false;
        PositionData? chosenPosition = null;
        //Go down the list of skills this enemy has
        foreach (Skill skill in skills)
        {
            if (skill.isReady)
            {
                // TODO: Add behavior for other skill types when implemented
                if (skill.SkillType != SkillType.Attack)
                {
                    continue;
                }

                List<Vector2Int> range = new List<Vector2Int>();
                List<Character_Combat> targets = new List<Character_Combat>();
                if (skill.SkillType == SkillType.Attack)
                {
                    AttackParam param = skill.skillData.param as AttackParam;
                    range = param.range;
                    foreach (PlayerCharacter_Combat character in CombatManager.Instance.playerCharacters_Combat)
                    {
                        targets.Add(character);
                    }
                }

                chosenPosition = GetTargetingTile(movableTiles, range, targets, skill.skillData.needAimingBeforeCast);

                if (chosenPosition != null)
                {
                    chosenSkill = skill;
                    break;
                }
            }
        }

        //If no skill could be used, try using the basic attack
        if (chosenSkill == null)
        {
            List<Character_Combat> targets = new List<Character_Combat>();
            foreach (PlayerCharacter_Combat character in CombatManager.Instance.playerCharacters_Combat)
            {
                targets.Add(character);
            }
            chosenPosition = GetTargetingTile(movableTiles, attackRange, targets, true);
            if (chosenPosition != null)
            {
                shouldBasicAttack = true;
            }
        }

        //If skill and basic attack both failed, just move closer to the closest character
        if (chosenPosition == null)
        {
            Vector2Int moveTowardsTile = Vector2Int.zero;
            int closestDistance = int.MaxValue;
            foreach (PlayerCharacter_Combat character in CombatManager.Instance.playerCharacters_Combat)
            {
                Vector2Int characterPosition = GridManager.Instance.PosToGrid(character.entity.transform.position);
                int distance = Math.Abs(currentPosition.x - characterPosition.x) + Math.Abs(currentPosition.y - characterPosition.y);
                if (distance < closestDistance) {
                    moveTowardsTile = characterPosition;
                    closestDistance = distance;
                }
            }

            closestDistance = int.MaxValue;
            foreach (Vector2Int movableTile in movableTiles)
            {
                int distance = Math.Abs(moveTowardsTile.x - movableTile.x) + Math.Abs(moveTowardsTile.y - movableTile.y);
                if (distance < closestDistance)
                {
                    Debug.Log(movableTile);
                    chosenPosition = new PositionData { tile = movableTile, rotation = 180 };
                    closestDistance = distance;
                }
            }
        }

        Vector2Int targetPosition = chosenPosition.Value.tile;
        if (currentPosition != targetPosition)
        {
            GridManager.Instance.Move(currentPosition.x, currentPosition.y, targetPosition.x, targetPosition.y);
        }
        entity.transform.rotation = Quaternion.Euler(0.0f, chosenPosition.Value.rotation, 0.0f);

        if (chosenSkill != null || shouldBasicAttack)
        {
            //Pause between enemy movement and ability usage
            yield return new WaitForSeconds(1.0f);

            if (chosenSkill != null)
            {
                chosenSkill.Execute(this);
            }
            else
            {
                GridManager.Instance.ApplyDamageToCells(this, TransformRangeToWorld(attackRange.ToArray()), ATK);
                CombatUI.Instance.UpdateCombatInfo();
            }
        }
    }
}