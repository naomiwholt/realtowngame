using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class ObjectPlacementManager : MonoBehaviour
{
    public static ObjectPlacementManager Instance { get; private set; }

    // Dictionary to store occupied tiles (key: grid position, value: true if occupied)
    private Dictionary<Vector3Int, bool> occupiedTiles = new Dictionary<Vector3Int, bool>();

    public TileBase whiteTile;  // Transparent white tile
    public TileBase orangeTile; // Transparent orange tile

    // List of objects already in the scene that need to be marked as occupying tiles
    public List<GameObject> preExistingObjects;
    public Tilemap tilemap;  // Reference to the Tilemap
    private HashSet<Vector3Int> occupiedtiles = new HashSet<Vector3Int>();  // Keep track of occupied cells

    public IsometricDepthSorting sortingManager;




    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;  // Subscribe to GameManager state change event
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;  // Unsubscribe to avoid memory leaks
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Playing)
        {
            Initialise();
            Debug.Log("initialising object placement");
        }
    }

    private void Initialise()
    {
        CollectFurnitureObjects();
        MarkPreExistingObjects();
      
    }


    private void CollectFurnitureObjects()
    {
        preExistingObjects = new List<GameObject>();

        // Find the "Furniture" parent object in the scene
        GameObject furnitureParent = GameObject.Find("Furniture");
        if (furnitureParent != null)
        {
            Debug.Log("adding furniture to preexisting objects");
            // Add each child of "Furniture" to the preExistingObjects list
            foreach (Transform child in furnitureParent.transform)
            {
                preExistingObjects.Add(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("No GameObject named 'Furniture' found in the scene.");
        }
    }


    // Step 1: Loop through each object and find its child colliders' bounds
    private void MarkPreExistingObjects()
    {
        foreach (var obj in preExistingObjects)
        {
            BoxCollider2D[] childColliders = obj.GetComponentsInChildren<BoxCollider2D>();
            foreach (var collider in childColliders)
            {
                // Step 2: Mark the tiles based on the collider bounds
                MarkObjectTilesAsOccupied(collider);
            }
        }
    }

    // Function to check if a cell is occupied
    public bool IsTileOccupied(Vector3Int tileposition)
    {
        return occupiedtiles.Contains(tileposition);
    }

    // Function to mark the tiles occupied by an object
    public void MarkObjectTilesAsOccupied(BoxCollider2D collider)
    {
        Vector3Int centerCell = tilemap.WorldToCell(collider.bounds.center);
        FloodFillFromCenter(collider, centerCell);  // Reuse the flood fill logic to mark occupied cells
    }

    // Reuse your existing FloodFillFromCenter method to update occupiedtiles
    private void FloodFillFromCenter(BoxCollider2D collider, Vector3Int centerCell)
    {
        // Directions to move in: right, up, left, down, and diagonals (checking all 8 neighbors)
        Vector3Int[] directions = new Vector3Int[]
        {
        new Vector3Int(1, 0, 0),   // Right
        new Vector3Int(0, 1, 0),   // Up
        new Vector3Int(-1, 0, 0),  // Left
        new Vector3Int(0, -1, 0),  // Down
        new Vector3Int(1, 1, 0),   // Up-Right
        new Vector3Int(-1, 1, 0),  // Up-Left
        new Vector3Int(1, -1, 0),  // Down-Right
        new Vector3Int(-1, -1, 0)  // Down-Left
        };

        // Queue to track tiles that need to be checked
        Queue<Vector3Int> cellsToCheck = new Queue<Vector3Int>();

        // Track which cells have been visited to avoid duplicates
        HashSet<Vector3Int> visitedCells = new HashSet<Vector3Int>();

        // Start with the center tile
        cellsToCheck.Enqueue(centerCell);
        visitedCells.Add(centerCell);

        // Debug log for collider information
     //   Debug.Log($"Collider Bounds (World): Center = {collider.bounds.center}, Size = {collider.bounds.size}, GameObject = {collider.gameObject.name}");

        // While there are still cells to check
        while (cellsToCheck.Count > 0)
        {
            Vector3Int currentCell = cellsToCheck.Dequeue();

            // Get the bounds of this tile
            Bounds tileBounds = GetTileBounds(currentCell);

            // Debug log for when each tile is visited
          //  Debug.Log($"Visiting Tile at {tileBounds.center} (World Position), Cell: {currentCell}");

            // Check if the tile overlaps with the collider using Physics2D.OverlapBox
            if (IsTileOverlappingWithCollider(collider, tileBounds))
            {
                // Debug log when a tile is marked as occupied
              //  Debug.Log($"Tile at {tileBounds.center} is Occupied. GameObject = {collider.gameObject.name}");

                // Mark the tile as occupied if it overlaps with the collider
                MarkTileAsOccupied(currentCell);

                // Add neighboring tiles to the queue to be checked (flood-fill)
                foreach (var direction in directions)
                {
                    Vector3Int adjacentCell = currentCell + direction;

                    // Only add the neighboring tile if it hasn't been visited yet
                    if (!visitedCells.Contains(adjacentCell))
                    {
                        cellsToCheck.Enqueue(adjacentCell);
                        visitedCells.Add(adjacentCell);  // Mark the cell as visited
                    }
                }
            }
        }
    }

    // Helper function to check if a tile overlaps with the collider using Physics2D.OverlapBox
    private bool IsTileOverlappingWithCollider(BoxCollider2D collider, Bounds tileBounds)
    {
        // Use Physics2D.OverlapBox to check if the tile overlaps with the rotated collider
        Collider2D[] results = new Collider2D[1];  // Array to store the result

        // The tile's center and size, converted to the collider's local space
        Vector2 tileCenter = tileBounds.center;
        Vector2 tileSize = tileBounds.size;

        // Perform the overlap check using the collider's rotation
        bool isOverlapping = Physics2D.OverlapBox(tileCenter, tileSize, collider.transform.eulerAngles.z, new ContactFilter2D(), results) > 0;

        // Debug log the result
       // Debug.Log(isOverlapping ? $"Overlap found for tile at {tileBounds.center}" : $"No overlap found for tile at {tileBounds.center}");

        return isOverlapping;
    }

    // Helper function to get the bounds of a tile
    private Bounds GetTileBounds(Vector3Int tileCell)
    {
        Vector3 tileWorldPosition = tilemap.GetCellCenterWorld(tileCell);
        Vector3 tileSize = tilemap.cellSize;  // Assuming uniform tile size

        // Create a new bounds for the tile based on its world position and size
        return new Bounds(tileWorldPosition, tileSize);
    }



    private void MarkTileAsOccupied(Vector3Int gridPosition)
    {
        if (!occupiedTiles.ContainsKey(gridPosition))
        {
            occupiedTiles.Add(gridPosition, true);
            tilemap.SetTile(gridPosition, orangeTile); // Color the tile as occupied
        }
    }

    public void UpdateSortingOrder(GameObject obj)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Convert object's world position to grid position
            Vector2Int gridPosition = EssentialsManager._instance.gridManager.ConvertWorldToGrid(obj.transform.position);

            // Use the Y-tile position to determine sorting order
            spriteRenderer.sortingOrder = Mathf.FloorToInt(-gridPosition.y * 10);
        }
    }



    public HashSet<Vector3Int> GetOccupiedTiles()
    {
        return occupiedtiles;  // Returning the HashSet of occupied tiles (stored in occupiedtiles)
    }



    public void ResetTile(Vector3Int gridPosition)
    {
        if (occupiedTiles.ContainsKey(gridPosition))
        {
            occupiedTiles.Remove(gridPosition);
            tilemap.SetTile(gridPosition, whiteTile);  // Option to Reset to white tile
        }
    }
}









