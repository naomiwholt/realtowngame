using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    private Grid grid;  // Unity's Grid component

    private GameObject player;  // Reference to the player

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        grid = GetComponent<Grid>();  // Get the Unity Grid component

        if (grid == null)
        {
            Debug.LogError("Grid component not found!");
        }
    }

    private void Start()
    {
        // Find the player object (assign this dynamically or use a tag to locate the player)
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }

    private void Update()
    {
        // Update the player's grid position every frame
        if (player != null && grid != null)
        {
            UpdatePlayerGridPosition(player.transform.position);
        }
    }

    // Convert world position to grid position and update
    public void UpdatePlayerGridPosition(Vector3 playerWorldPosition)
    {
        Vector2Int playerGridPosition = ConvertWorldToGrid(playerWorldPosition);
        Debug.Log("Player grid position: " + playerGridPosition);
    }

    public Vector2Int ConvertWorldToGrid(Vector3 worldPosition)
    {
        if (grid == null)
        {
            Debug.LogWarning("Grid component is missing or has been destroyed.");
            return Vector2Int.zero;  // Handle the case where grid is missing
        }

        Vector3Int cellPosition = grid.WorldToCell(worldPosition);
        return new Vector2Int(cellPosition.x, cellPosition.y);
    }
}



