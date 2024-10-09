using System.Collections.Generic;
using UnityEngine;

public class SortingOrderController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private GridManager gridManager;
    private ObjectPlacementManager objectPlacementManager;  // Reference to ObjectPlacementManager
    public int sortingOrderOffset = 100;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridManager = GridManager.Instance;
        objectPlacementManager = FindObjectOfType<ObjectPlacementManager>();  // Get the ObjectPlacementManager
    }

    void LateUpdate()
    {
        // Get the player's current tile position
        Vector2Int playerTilePosition = gridManager.ConvertWorldToGrid(transform.position);

        // Calculate sorting order based on comparison with object tiles
        int sortingOrder = CalculateSortingOrder(playerTilePosition);

        // Set the sorting order with an offset
        spriteRenderer.sortingOrder = sortingOrder + sortingOrderOffset;
    }

    // Method to calculate sorting order based on occupied tiles of objects
    private int CalculateSortingOrder(Vector2Int playerTilePosition)
    {
        int baseSortingOrder = -(playerTilePosition.y * 10); // Base sorting order from player's tile position

        // Get all occupied tiles from ObjectPlacementManager
        HashSet<Vector3Int> occupiedTiles = objectPlacementManager.GetOccupiedTiles();

        foreach (var occupiedTile in occupiedTiles)
        {
            // Compare player’s tile position to the occupied tile
            if (playerTilePosition.y < occupiedTile.y)
            {
                // Player is behind the object, adjust the sorting order to draw the object in front
                baseSortingOrder -= 5;
            }
            else if (playerTilePosition.y > occupiedTile.y)
            {
                // Player is in front of the object, adjust sorting to draw the player in front
                baseSortingOrder += 5;
            }
        }

        return baseSortingOrder;
    }

}


