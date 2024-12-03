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

    public void StartDrag(GameObject furniturePrefab)
    {
        Debug.Log("Start drag method called");
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

            // Ensure there is a BoxCollider2D component
            if (previewObject.GetComponent<BoxCollider2D>() == null)
            {
                Debug.Log("No collider on preview object.");
            }
        }
        else
        {
            Debug.LogWarning("Furniture prefab is null. Cannot start drag.");
        }
    }

    public void UpdateDragPosition(Vector2 screenPosition)
    {
        if (previewObject != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;
            previewObject.transform.position = worldPosition;

            BoxCollider2D collider = previewObject.GetComponentInChildren<BoxCollider2D>();
            if (collider != null)
            {
                bool isValid = !IsOverlappingWithOtherObjects(collider);
                SpriteRenderer previewRenderer = previewObject.GetComponent<SpriteRenderer>();
                if (previewRenderer != null)
                {
                    previewRenderer.color = isValid ? previewValidColor : previewInvalidColor;
                }
            }
        }
    }

    private bool IsOverlappingWithOtherObjects(BoxCollider2D collider)
    {
        Collider2D[] results = new Collider2D[10];
        int count = Physics2D.OverlapCollider(collider, new ContactFilter2D(), results);

        // Check if any of the colliders in results are not the original collider
        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i] != collider)
            {
                return true; // Overlapping with another object
            }
        }
        return false; // No overlaps with other objects
    }

    public void EndDrag(Vector2 screenPosition, InventoryItemData itemData, InventorySlot slot)
    {
        if (previewObject != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;

            BoxCollider2D collider = previewObject.GetComponentInChildren<BoxCollider2D>();
            if (collider != null && !IsOverlappingWithOtherObjects(collider))
            {
                slot.ClearSlot();
                EssentialsManager._instance.inventoryManager.RemoveItem(itemData);
                PlaceFurniture(itemData.itemPrefab, worldPosition);
            }
            else
            {
                Debug.LogWarning("Cannot place furniture; it overlaps with another object.");
            }
            ClearPreview();
            Destroy(previewObject);
        }
    }

    public void ClearPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    public void PlaceFurniture(GameObject furniturePrefab, Vector3 worldPosition)
    {
        Debug.Log("Furniture placing method called");
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


        //FIX THIS just needs to be updated for dynamic sorting we actually should probabbly just call the initialise sorting method again in depth manabger

     //   EssentialsManager._instance.sortingManager.AddToSortingList(furnitureInstanceRenderer);
     //   EssentialsManager._instance.sortingManager.SortSprites();
    }
}









