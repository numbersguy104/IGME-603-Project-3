using System;
using UnityEngine;

//Manage the grid system for combat.
//Documentation: https://docs.google.com/document/d/1HdIbCRw4Lso9VstncTPQ-jHqG9BYdMHayuyans9pvLk/edit?usp=sharing
public class GridManager : MonoBehaviour
{
    private GameObject[,] grid;
    [SerializeField] private GameObject tilePrefab;

    //TEMPORARY: Initialize the grid on startup
    //Usually Init() should be called externally for customizable grid size
    private void Start()
    {
        Init(10, 10);
    }

    //Initialize the grid
    //This should ALWAYS be called as soon as possible when setting up the grid!
    //Otherwise the array will not be set up properly.
    public void Init(int sizeX, int sizeY)
    {
        grid = new GameObject[sizeX, sizeY];
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
                Vector3 tilePos = new Vector3(x + offset.x, 0.0f, y + offset.y);
                tile.transform.localPosition = tilePos;

                if ((x + y) % 2 == 1)
                {
                    tile.GetComponent<MeshRenderer>().material.color = new Color(0.9f, 0.9f, 0.9f);
                }
            }
        }
    }
}