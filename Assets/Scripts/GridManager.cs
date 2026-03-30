using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

//Manage the grid system for combat.
//Documentation: https://docs.google.com/document/d/1HdIbCRw4Lso9VstncTPQ-jHqG9BYdMHayuyans9pvLk/edit?usp=sharing

public enum HighlightType
{
    InAttackingRange,
    InBuffRange,
    Hovered,
    InMovingRange
}

public struct GridHighlight
{
    public Vector2Int[] positions;
    public HighlightType type;

    public GridHighlight(Vector2Int[] positions, HighlightType type)
    {
        this.positions = positions;
        this.type = type;
    }
}
public class GridManager : SingletonBehavior<GridManager>
{
    private GameObject[,] grid;
    private GridTile[,] tiles;
    public Vector2Int Size;
    public int[,] highlightMap;
    private List<GridHighlight> highlightList =  new List<GridHighlight>();
    [SerializeField] private GameObject tilePrefab;

    //TEMPORARY: Initialize the grid on startup
    //Usually Init() should be called externally for customizable grid size
    private void Start()
    {
        // Init(10, 10);
    }

    //Initialize the grid
    //This should ALWAYS be called as soon as possible when setting up the grid!
    //Otherwise the array will not be set up properly.
    public void Init(int sizeX, int sizeY)
    {
        grid = new GameObject[sizeX, sizeY];
        tiles = new GridTile[sizeX, sizeY];
        Size = new Vector2Int(sizeX, sizeY);
        CreateTiles(sizeX, sizeY);
    }
    
    //Converts a world position vector to coordinates on the grid
    //The height (Y-axis) of the input is ignored; only X and Z are used
    //If bounded is true (default), locations off of the grid are clamped to an edge or corner
    public Vector2Int PosToGrid(Vector3 worldPosition, bool bounded = true)
    {
        Vector2 offset = GetOffset();
        int x = (int)Math.Round(worldPosition.x - offset.x);
        int y = (int)Math.Round(worldPosition.z - offset.y);
        if (bounded)
        {
            x = Math.Clamp(x, 0, grid.GetLength(0));
            y = Math.Clamp(y, 0, grid.GetLength(1));
        }
        return new Vector2Int(x, y);
    }

    //Converts coordinates on the grid to a world position at the center of that grid space
    //The height (Y-axis) is set to 0
    //If bounded is true (default), locations off of the grid are clamped to an edge or corner
    public Vector3 GridToPos(Vector2Int gridCoordinates, bool bounded = true)
    {
        Vector2 offset = GetOffset();
        int x = gridCoordinates.x;
        int y = gridCoordinates.y;
        if (bounded)
        {
            x = Math.Clamp(x, 0, grid.GetLength(0));
            y = Math.Clamp(y, 0, grid.GetLength(1));
        }
        float worldX = x + offset.x;
        float worldZ = y + offset.y;
        return new Vector3(worldX, 0.0f, worldZ);
    }

    //Returns whether an object is on the grid at coordinates (x, y)
    public bool IsOccupied(int x, int y)
    {
        return (grid[x,y] != null);
    }

    //Returns the object at coordinates (x, y) on the grid
    //Returns null if there's nothing there
#nullable enable
    public GameObject? GetAt(int x, int y)
    {
        if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
            return null;
        return grid[x,y];
    }
#nullable disable

    //Gets an arbitrary GameObject on the grid that has the provided component
    //If none are found, returns null
#nullable enable
    public GameObject? GetByComponent<T>() {
        foreach (GameObject o in grid) {
            if (o.GetComponent<T>() != null)
            {
                return o;
            }
        }
        return null;
    }
#nullable disable

    //Gets the grid coordinates of an arbitrary GameObject with the provided component
    //If none are found, returns null
#nullable enable
    public Vector2Int? GetCoordinatesByComponent<T>()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                GameObject o = grid[x, y];
                if (o != null && o.GetComponent<T>() != null)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }
#nullable disable

    //Adds a preexisting object to the grid at coordinates (x, y)
    //If shouldMove is true (default), also moves the object's transform there
    //Does nothing and returns false if an object is already there
    //Otherwise, returns true
    public bool Add(GameObject o, int x, int y, bool shouldMove = true)
    {
        if (IsOccupied(x, y))
        {
            return false;
        }
        grid[x,y] = o;
        if (shouldMove)
        {
            o.transform.position = GridToPos(new Vector2Int(x, y));
        }
        return true;
    }

    //Removes the object at coordinates (x, y) on the grid
    //If shouldDestroy is true (default), also destroys that GameObject
    //Returns false if there is no object there to remove
    //Otherwise, returns true
    public bool RemoveAt(int x, int y, bool shouldDestroy = true) { 
        if (!IsOccupied(x, y))
        {
            return false;
        }
        if (shouldDestroy)
        {
            Destroy(grid[x,y]);
        }
        grid[x,y] = null;
        return true;
    }

    //Moves an object at grid coordinates (fromX, fromY) to (toX, toY)
    //If shouldMove is true (default), also moves the object's transform
    //Does nothing and returns false if:
    // There is no object at (fromX, fromY), or
    // There is already an object at (toX, toY)
    //Otherwise, returns true
    public bool Move(int fromX, int fromY, int toX, int toY, bool shouldMove = true)
    {
        if (IsOccupied(toX, toY))
        {
            return false;
        }
        GameObject target = GetAt(fromX, fromY);
        if (target == null)
        {
            return false;
        }
        RemoveAt(fromX, fromY, false);
        Add(target, toX, toY, shouldMove);
        return true;
    }

    //Removes all objects that have the provided component from the grid
    //If shouldDestroy is true (default), also destroys the GameObjects
    //Returns the number of objects that were removed
    public int RemoveAllByComponent<T>(bool shouldDestroy = true)
    {
        int removedCount = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                GameObject o = grid[x,y];
                if (o != null && o.GetComponent<T>() != null)
                {
                    if (shouldDestroy)
                    {
                        Destroy(o);
                    }
                    grid[x,y] = null;
                    removedCount++;
                }
            }
        }
        return removedCount;
    }

    //Get the "offset" from the grid object to the (0, 0) corner of the grid
    //Used to ensure everything is centered on the grid object
    private Vector2 GetOffset()
    {
        float ox = -(grid.GetLength(0) - 1) / 2.0f;
        float oy = -(grid.GetLength(1) - 1) / 2.0f;
        return new Vector2(ox, oy);
    }

    //Creates the visual tile graphics associated with the grid
    private void CreateTiles(int sizeX, int sizeY)
    {
        Vector2 offset = GetOffset();
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                GameObject tile = Instantiate(tilePrefab, transform);
                tiles[x,y] = tile.GetComponent<GridTile>();
                Vector3 tilePos = new Vector3(x + offset.x, 0.0f, y + offset.y);
                tile.transform.localPosition = tilePos;

                if ((x + y) % 2 == 1)
                {
                    tile.GetComponent<MeshRenderer>().material.SetFloat("_Type", 1);
                }
            }
        }
    }
    
    /// <summary>
    /// Get the current tile the mouse if pointing at.
    /// </summary>
    /// <returns>The coordinate of the tile. Null if cursor pointing at nothing</returns>
    #nullable enable
    public Vector2Int? GetHoveredTile(bool shouldHighlight)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return PosToGrid(hit.point, false);
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
        }

        return null;
    }
    #nullable disable
    
    /// <summary>
    /// Apply damage to cells on the grid.
    /// </summary>
    /// <param name="instigator">The character that deals this damage</param>
    /// <param name="range">The coordinates of the cells to be affected</param>
    /// <param name="dmg">The damage to be done</param>
    /// <param name="onDamageDealt">An action for post-damage event</param>

    public void ApplyDamageToTiles(Character_Combat instigator, Vector2Int[] range, float dmg, Action<Character_Combat, Character_Combat, float> onDamageDealt = null)
    {
        foreach (var coor in range)
        {
            int x = coor.x;
            int y = coor.y;
            if (GetAt(x, y)!= null && GetAt(x, y).TryGetComponent<CombatEntity>(out var entity))
            {
                Character_Combat character = entity.character;
                if (character.team != instigator.team)
                {
                    character.TakeDamage(dmg);
                    onDamageDealt?.Invoke(instigator, character, dmg);
                }
            }
        }
    }
    
    public void ApplyEffectToTiles(Character_Combat instigator, Vector2Int[] range, BuffParam.EffectByChances effectByChances, bool applyOnAlly)
    {
        foreach (var coor in range)
        {
            int x = coor.x;
            int y = coor.y;
            if (GetAt(x, y)!= null && GetAt(x, y).TryGetComponent<CombatEntity>(out var entity))
            {
                Character_Combat character = entity.character;
                if((character.team == instigator.team) == applyOnAlly)
                    if(Random.Range(0,1f) < effectByChances.chance)
                    {
                        StatusWithTurns statusWithTurns = effectByChances.statusWithTurns;
                        character.AddStatus(statusWithTurns.status, statusWithTurns.turns);
                    }
            }
        }
    }

    //Get tiles within a certain range of the origin
    //If ignoreObstacles is true, obstacles (characters and pits) will not be considered
    public Vector2Int[] GetTilesWithinRange(Vector2Int origin, int range, bool ignoreObstacles = false)
    {
        List<Vector2Int> tilesInRange = new List<Vector2Int>();

        if (ignoreObstacles)
        {
            for (int x = -range; x <= range; x++)
                for (int y = -range; y <= range; y++)
                {
                    int X = origin.x + x;
                    int Y = origin.y + y;
                    if (X >= 0 && X < grid.GetLength(0) && Y >= 0 && Y < grid.GetLength(1))
                        if (Math.Abs(x) + Math.Abs(y) <= range)
                            tilesInRange.Add(new Vector2Int(X, Y));
                }
        }
        else
        {
            //Use Dijkstra's algorithm to calculate tile costs
            Dictionary<Vector2Int, int> unvisited = new Dictionary<Vector2Int, int>();
            Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();

            //Start by filling in the "unvisited set"
            //Only need to consider tiles within range, not the whole grid
            for (int x = origin.x - range; x <= origin.x + range; x++)
            {
                for (int y = origin.y - range; y <= origin.y + range; y++)
                {
                    
                    //TODO: Also check for non-character obstacles (pits) here, if needed // Shaolin: Added a check for player characters so that they won't block each other
                    if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1) && (GetAt(x, y) == null || GetAt(x,y).CompareTag("Player"))) 
                    {
                        unvisited.Add(new Vector2Int(x, y), int.MaxValue);
                    }
                }
            }
            unvisited[origin] = 0;

            //Helper vectors for checking tile adjacency
            Vector2Int[] unitVectors = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

            //Main Dijkstra loop
            while (unvisited.Count > 0)
            {
                //Select the next tile to check
                Vector2Int nextTile = Vector2Int.zero;
                int lowestDistance = int.MaxValue;
                foreach (KeyValuePair<Vector2Int, int> pair in unvisited)
                {
                    if (pair.Value < lowestDistance)
                    {
                        lowestDistance = pair.Value;
                        nextTile = pair.Key;
                    }
                }

                //Break the loop if all remaining tiles are unreachable
                if (lowestDistance == int.MaxValue)
                {
                    break;
                }

                //Update distances to each neighboring tile
                foreach (Vector2Int adjacentVector in unitVectors)
                {
                    Vector2Int adjacentTile = nextTile + adjacentVector;

                    if (unvisited.ContainsKey(adjacentTile))
                    {
                        unvisited[adjacentTile] = lowestDistance + 1;
                    }
                }

                visited.Add(nextTile, unvisited[nextTile]);
                unvisited.Remove(nextTile);
            }

            //Return tiles at or below the specified cost
            foreach (KeyValuePair<Vector2Int, int> pair in visited)
            {
                if (pair.Value <= range)
                {
                    tilesInRange.Add(pair.Key);
                }
            }
        }

        return tilesInRange.ToArray();
    }

    public bool CheckOnBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < grid.GetLength(0) && pos.y >= 0 && pos.y < grid.GetLength(1);
    }

    public void AddHighlight(GridHighlight highlight)
    {
        highlightList.Add(highlight);
        RefreshHighlight();
    }
    
    public void RemoveHighlight(GridHighlight highlight)
    {
        highlightList.Remove(highlight);
        RefreshHighlight();
    }
    
    public void RefreshHighlight()
    {
        highlightMap = new int[Size.x, Size.y];
        foreach (var highlight in highlightList)
        {
            foreach (var coor in highlight.positions)
            {
                if (coor.x >= 0 && coor.x < Size.x && coor.y >= 0 && coor.y < Size.y)
                    highlightMap[coor.x, coor.y] |= 1 << (int)highlight.type;
            }
        }
        
        for (int x = 0; x < Size.x; x++)
            for (int y = 0; y < Size.y; y++)
            {
                tiles[x, y].SetHighlight(highlightMap[x, y]);
            }
    }
}