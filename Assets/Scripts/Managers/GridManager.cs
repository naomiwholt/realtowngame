using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public TileBase dustyTile; // Placeholder for dusty tiles
    public TileBase cleanTile; // Placeholder for clean tiles
    public Tilemap tilemap;

    private void Awake()
    {
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap is missing. Please assign it in the inspector.");
        }
    }

    // initialised in the essentials manager
    public void InitialiseDustyFloor()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tilemap.SetTile(pos, dustyTile);
            }
        }
    }

    // Clear a tile (vacuum mechanic)
    public void ClearTile(Vector3Int tilePosition)
    {
        if (tilemap.GetTile(tilePosition) == dustyTile)
        {
            tilemap.SetTile(tilePosition, cleanTile);
           // Debug.Log($"Tile at {tilePosition} cleaned!");
        }
    }

    // Check if a tile is dusty
    public bool IsDusty(Vector3Int tilePosition)
    {
        return tilemap.GetTile(tilePosition) == dustyTile;
    }
}



