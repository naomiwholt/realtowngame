using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPlacementManager : MonoBehaviour
{
    // Scene objects
    public List<GameObject> preExistingObjects;
    public DepthSortingManager sortingManager;

    private GameObject previewObject;
    private Color previewValidColor = new Color(1, 1, 1, 0.5f); // semi-transparent white
    private Color previewInvalidColor = new Color(1, 0, 0, 0.5f); // semi-transparent red

    public void Initialise()
    {
        CollectFurnitureObjects();
        MarkPreExistingObjects();
    }

    private void CollectFurnitureObjects()
    {
        preExistingObjects = new List<GameObject>();

        GameObject furnitureParent = GameObject.Find("Furniture");
        if (furnitureParent != null)
        {
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

    private void MarkPreExistingObjects()
    {
        foreach (var obj in preExistingObjects)
        {
            BoxCollider2D[] childColliders = obj.GetComponentsInChildren<BoxCollider2D>();
            foreach (var collider in childColliders)
            {
                EssentialsManager._instance.gridManager.MarkObjectTilesAsOccupied(collider);
            }
        }
    }

    public void StartDrag(GameObject furniturePrefab)
    {
        if (furniturePrefab != null)
        {
            // Instantiate a preview object for visual feedback
            previewObject = Instantiate(furniturePrefab);

            // Set the preview object's sorting layer to "GameWorld"
            SpriteRenderer previewRenderer = previewObject.GetComponent<SpriteRenderer>();
            if (previewRenderer != null)
            {
                previewRenderer.color = previewValidColor; // Set transparency
                previewRenderer.sortingLayerName = "GameWorld"; // Ensure correct layer
            }
        }
    }


    public void UpdateDragPosition(Vector2 screenPosition)
    {
        if (previewObject != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;
            previewObject.transform.position = worldPosition;

            Vector3Int gridPosition = EssentialsManager._instance.gridManager.tilemap.WorldToCell(worldPosition);
            bool isValid = !EssentialsManager._instance.gridManager.IsTileOccupied(gridPosition);

            // Update the preview color to indicate placement validity
            previewObject.GetComponent<SpriteRenderer>().color = isValid ? previewValidColor : previewInvalidColor;
        }
    }

    public void EndDrag(Vector2 screenPosition, GameObject furniturePrefab, InventorySlot slot)
    {
        if (previewObject != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;

            // Validate that the furniture's collider is within valid tiles
            BoxCollider2D collider = previewObject.GetComponentInChildren<BoxCollider2D>();
            if (collider != null && EssentialsManager._instance.gridManager.ValidateColliderWithinValidTiles(collider))
            {
                // Place furniture in the game world if the position is valid
                PlaceFurniture(furniturePrefab, worldPosition);
                EssentialsManager._instance.inventoryManager.RemoveItem(furniturePrefab);

                // Clear the inventory slot on successful placement
                slot.ClearSlot();
            }
            else
            {
                Debug.LogWarning("Cannot place furniture; part of the object is outside valid tiles or overlaps an occupied tile.");
            }
            Destroy(previewObject); // Remove the preview object
        }
    }








    public void PlaceFurniture(GameObject furniturePrefab, Vector3 worldPosition)
    {
        if (furniturePrefab == null)
        {
            Debug.LogWarning("Furniture prefab is null. Cannot place furniture.");
            return;
        }

        Scene currentGameplayScene = EssentialsManager._instance.sceneController.GetCurrentGameplayScene();
        if (!currentGameplayScene.IsValid())
        {
            Debug.LogError("No valid gameplay scene found to place furniture.");
            return;
        }

        Vector3Int gridPosition = EssentialsManager._instance.gridManager.tilemap.WorldToCell(worldPosition);
        if (EssentialsManager._instance.gridManager.IsTileOccupied(gridPosition))
        {
            Debug.LogWarning("Cannot place furniture here, the tile is already occupied.");
            return;
        }

        GameObject furnitureInstance = Instantiate(furniturePrefab, worldPosition, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(furnitureInstance, currentGameplayScene);

        GameObject furnitureParent = GameObject.Find("Furniture");
        if (furnitureParent != null)
        {
            furnitureInstance.transform.SetParent(furnitureParent.transform);
        }

        furnitureInstance.layer = LayerMask.NameToLayer("InteractableObject");
        SpriteRenderer furnitureInstanceRenderer = furnitureInstance.GetComponent<SpriteRenderer>();
        furnitureInstanceRenderer.sortingLayerName = "GameWorld";
        EssentialsManager._instance.sortingManager.AddToSortingList(furnitureInstanceRenderer);
        EssentialsManager._instance.sortingManager.SortSprites();

        BoxCollider2D collider = furnitureInstance.GetComponentInChildren<BoxCollider2D>();
        if (collider != null)
        {
            EssentialsManager._instance.gridManager.MarkObjectTilesAsOccupied(collider);
        }
        else
        {
            EssentialsManager._instance.gridManager.MarkTileAsOccupied(gridPosition);
        }
    }
}











