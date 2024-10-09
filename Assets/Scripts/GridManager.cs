using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    private Grid grid;  // Unity's Grid component

    [Header("Grid Settings")]
    public int gridWidth = 200;
    public int gridHeight = 200;

    public bool buildMode = false;  // Toggle build mode

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        grid = GetComponent<Grid>();   // Get the Unity Grid component

    }

    public Vector3 ConvertGridToWorld(Vector2Int gridPosition)
    {
        return grid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
    }

    // Convert world position to grid position
    public Vector2Int ConvertWorldToGrid(Vector3 worldPosition)
    {
        Vector3Int cellPosition = grid.WorldToCell(worldPosition);
        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

  
}

