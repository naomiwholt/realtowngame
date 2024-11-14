using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    private HashSet<Vector3Int> occupiedtiles = new HashSet<Vector3Int>();
    public TileBase whiteTile; // Unoccupied tile color
    public TileBase orangeTile; // Occupied tile color
    public TileBase yellowTile; // Preview tile color
    public Tilemap tilemap;

    private Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap is missing. Please assign it in the inspector.");
        }
    }

    // Method to mark all tiles within the bounds of a collider
    public void MarkTilesWithinColliderBounds(BoxCollider2D collider)
    {
        Bounds bounds = collider.bounds;
        Vector3Int minCell = tilemap.WorldToCell(bounds.min);
        Vector3Int maxCell = tilemap.WorldToCell(bounds.max);

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = tilemap.CellToWorld(cellPosition);

                if (collider.OverlapPoint(worldPosition))
                {
                    MarkTileAsOccupied(cellPosition);
                }
            }
        }
    }

    // Check if a tile is occupied
    public bool IsTileOccupied(Vector3Int tilePosition)
    {
        return occupiedtiles.Contains(tilePosition);
    }

    // Check if the collider fits entirely within valid, unoccupied white tiles
    public bool ValidateColliderWithinValidTiles(BoxCollider2D collider)
    {
        Vector3Int centerCell = tilemap.WorldToCell(collider.bounds.center);
        return FloodFillFromCenter(collider, centerCell, true);
    }

    public void MarkObjectTilesAsOccupied(BoxCollider2D collider)
    {
        Vector3Int centerCell = tilemap.WorldToCell(collider.bounds.center);
        FloodFillFromCenter(collider, centerCell, false);
    }

    // Mark a specific tile as occupied
    public void MarkTileAsOccupied(Vector3Int gridPosition)
    {
        if (occupiedtiles.Add(gridPosition))
        {
            tilemap.SetTile(gridPosition, orangeTile);
        }
    }

    // Reset a tile to unoccupied
    public void ResetTile(Vector3Int gridPosition)
    {
        if (occupiedtiles.Remove(gridPosition))
        {
            tilemap.SetTile(gridPosition, whiteTile);
        }
    }

    private HashSet<Vector3Int> previousPreviewTiles = new HashSet<Vector3Int>();

    public void PreviewTilesWithinColliderBounds(BoxCollider2D collider, bool showPreview)
    {
        Bounds bounds = collider.bounds;
        Vector3Int minCell = tilemap.WorldToCell(bounds.min);
        Vector3Int maxCell = tilemap.WorldToCell(bounds.max);

        HashSet<Vector3Int> currentPreviewTiles = new HashSet<Vector3Int>();

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = tilemap.CellToWorld(cellPosition);

                if (collider.OverlapPoint(worldPosition))
                {
                    currentPreviewTiles.Add(cellPosition);
                    if (showPreview)
                    {
                        tilemap.SetTile(cellPosition, yellowTile);
                    }
                }
            }
        }

        if (showPreview)
        {
            // Clear previously previewed tiles that are no longer in the current preview set
            foreach (Vector3Int cell in previousPreviewTiles)
            {
                if (!currentPreviewTiles.Contains(cell))
                {
                    // Reset the tile back to whiteTile instead of null
                    tilemap.SetTile(cell, whiteTile);
                }
            }
            previousPreviewTiles = currentPreviewTiles;
        }
        else
        {
            // Clear all preview tiles by setting them back to whiteTile
            foreach (Vector3Int cell in previousPreviewTiles)
            {
                tilemap.SetTile(cell, whiteTile);
            }
            previousPreviewTiles.Clear();
        }
    }
    // Flood fill starting from a center cell, marking overlapping tiles
    private bool FloodFillFromCenter(BoxCollider2D collider, Vector3Int centerCell, bool validateOnly)
    {
        Vector3Int[] directions = {
        new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0),
        new Vector3Int(1, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, -1, 0)
    };

        var cellsToCheck = new Queue<Vector3Int>(new[] { centerCell });
        var visitedCells = new HashSet<Vector3Int> { centerCell };
        bool allTilesValid = true;

        while (cellsToCheck.Count > 0)
        {
            var currentCell = cellsToCheck.Dequeue();
            if (!IsTileOverlappingWithCollider(collider, GetTileBounds(currentCell))) continue;

            TileBase tile = tilemap.GetTile(currentCell);
            bool isValidTile = tile == whiteTile && !IsTileOccupied(currentCell);

            if (validateOnly)
            {
                // For validation, check if tile is invalid and mark for feedback
                if (!isValidTile)
                {
                    allTilesValid = false;
                    break;
                }
                tilemap.SetTile(currentCell, isValidTile ? orangeTile : whiteTile); // Visual feedback for testing
            }
            else
            {
                // For marking, set tiles as occupied
                MarkTileAsOccupied(currentCell);
            }

            foreach (var direction in directions)
            {
                Vector3Int adjacentCell = currentCell + direction;
                if (visitedCells.Add(adjacentCell))
                    cellsToCheck.Enqueue(adjacentCell);
            }
        }

        // Reset temporary feedback to white after validation
        if (validateOnly)
        {
            foreach (var cell in visitedCells)
            {
                tilemap.SetTile(cell, whiteTile);
            }
        }

        return allTilesValid;
    }
    // Check if a tile overlaps with a collider
    private bool IsTileOverlappingWithCollider(BoxCollider2D collider, Bounds tileBounds)
    {
        Collider2D[] results = new Collider2D[1];
        Vector2 tileCenter = tileBounds.center;
        Vector2 tileSize = tileBounds.size;
        return Physics2D.OverlapBox(tileCenter, tileSize, collider.transform.eulerAngles.z, new ContactFilter2D(), results) > 0;
    }

    // Get the bounds of a tile at a given cell position
    private Bounds GetTileBounds(Vector3Int tileCell)
    {
        Vector3 tileWorldPosition = tilemap.GetCellCenterWorld(tileCell);
        Vector3 tileSize = tilemap.cellSize;
        return new Bounds(tileWorldPosition, tileSize);
    }
}






