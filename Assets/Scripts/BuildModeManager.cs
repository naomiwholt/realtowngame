using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;  // New Input System

public class BuildModeManager : MonoBehaviour
{
    public Tilemap tilemap;  // Reference to the Tilemap
    public LayerMask selectableLayer;  // Layer for objects that can be selected
    private GameObject selectedObject;  // Currently selected object
    private Vector3 offset;  // Offset for dragging

    private bool isDragging = false;
    private bool isBuildModeActive = false;  // Toggle for build mode

    private ObjectPlacementManager placementManager;  // Reference to the ObjectPlacementManager script

    private void Start()
    {
        placementManager = FindObjectOfType<ObjectPlacementManager>();  // Get reference to ObjectPlacementManager
    }

    void Update()
    {
        // Toggle build mode on/off with 'B' key (using Input System)
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleBuildMode();
        }

        if (isBuildModeActive)
        {
            HandleSelection();
            HandleDragging();
            HandlePlacement();
        }
    }

    // Toggle build mode
    private void ToggleBuildMode()
    {
        isBuildModeActive = !isBuildModeActive;

        if (isBuildModeActive)
        {
            Debug.Log("Build Mode Activated");
        }
        else
        {
            Debug.Log("Build Mode Deactivated");
            selectedObject = null;  // Deselect any selected object when exiting build mode
        }
    }

    // Handle selecting an object
    void HandleSelection()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, selectableLayer);

            if (hit.collider != null)
            {
                selectedObject = hit.collider.gameObject;
                offset = selectedObject.transform.position - mouseWorldPos;
                isDragging = true;

                Debug.Log("Selected: " + selectedObject.name);
            }
        }
    }

    // Handle dragging the selected object
    void HandleDragging()
    {
        if (selectedObject != null && isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 newPosition = mouseWorldPos + offset;

            // Snap object to the grid
            Vector3Int tileposition = tilemap.WorldToCell(newPosition);

            // Check if the cell is occupied
            if (!placementManager.IsTileOccupied(tileposition))
            {
                selectedObject.transform.position = tilemap.GetCellCenterWorld(tileposition);
            }
            else
            {
                Debug.Log("Cell is occupied, can't place the object here!");
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
        }
    }

    // Handle placing the selected object
    void HandlePlacement()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedObject != null)
        {
            // Check if the object can be placed in the current position
            Vector3Int cellPosition = tilemap.WorldToCell(selectedObject.transform.position);

            if (!placementManager.IsTileOccupied(cellPosition))
            {
                // Mark the object's position as occupied in the grid
                BoxCollider2D collider = selectedObject.GetComponentInChildren<BoxCollider2D>();
                if (collider != null)
                {
                    placementManager.MarkObjectTilesAsOccupied(collider);
                }

                Debug.Log("Placed: " + selectedObject.name);
                selectedObject = null;  // Deselect object after placing
            }
            else
            {
                Debug.Log("Can't place the object here, cell is occupied!");
            }
        }
    }
}




