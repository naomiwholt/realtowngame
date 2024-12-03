using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ObjectPlacementManager : MonoBehaviour
{
    // Scene objects
    public List<GameObject> preExistingObjects;
    public DepthSortingManager sortingManager;

    private GameObject currentSelectedObject;
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

    public void StartDrag(InventoryItemData itemData)
    {
        Debug.Log("Start drag method called");
        if (itemData != null && itemData.itemPrefab != null)
        {
            // Instantiate a preview object for visual feedback
            currentSelectedObject = Instantiate(itemData.itemPrefab);

            // Set the preview object's sorting layer to "GameWorld"
            SpriteRenderer previewRenderer = currentSelectedObject.GetComponent<SpriteRenderer>();
            if (previewRenderer != null)
            {
                previewRenderer.color = previewValidColor; // Set transparency
                previewRenderer.sortingLayerName = "GameWorld"; // Ensure correct layer
            }

            // Ensure there is a BoxCollider2D component
            if (currentSelectedObject.GetComponent<PolygonCollider2D>() == null)
            {
                Debug.Log("No collider on preview object.");
            }
        }
        else
        {
            Debug.LogWarning("ItemData or itemPrefab is null. Cannot start drag.");
        }
    }


    public void UpdateDragPosition(Vector2 screenPosition, InventoryItemData itemData)
    {

        if (currentSelectedObject != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;
            currentSelectedObject.transform.position = worldPosition;
            Debug.Log("Updating drag position");    

            // Handle right-click rotation using Input System
            if (Mouse.current.rightButton.wasPressedThisFrame && itemData.canRotate)
            {
                itemData.RotateItem(currentSelectedObject);
                Debug.Log("Rotating item");
            }

            // Validate placement
            PolygonCollider2D collider = currentSelectedObject.GetComponent<PolygonCollider2D>();
            if (collider != null)
            {
                bool isValid = !IsOverlappingWithOtherObjects(collider);
                SpriteRenderer previewRenderer = currentSelectedObject.GetComponent<SpriteRenderer>();
                if (previewRenderer != null)
                {
                    previewRenderer.color = isValid ? previewValidColor : previewInvalidColor;
                }
            }
        }
        else
        {
            Debug.LogWarning("No currentSelectedObject found. Cannot update drag position.");
        }
    }



    private bool IsOverlappingWithOtherObjects(PolygonCollider2D collider)
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
        if (currentSelectedObject != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;

            PolygonCollider2D collider = currentSelectedObject.GetComponentInChildren<PolygonCollider2D>();
            if (collider != null && !IsOverlappingWithOtherObjects(collider))
            {
                slot.ClearSlot();
                EssentialsManager._instance.inventoryManager.RemoveItem(itemData);
                PlaceFurniture(itemData.itemPrefab, worldPosition);
                EssentialsManager._instance.sortingManager.InitialiseSorting();
            }
            else if (collider != null)
            {
                Debug.LogWarning("Cannot place furniture; as no collider on preview opbject");
            }
            ClearPreview();
            Destroy(currentSelectedObject);
        }
    }

    public void ClearPreview()
    {
        if (currentSelectedObject != null)
        {
            Destroy(currentSelectedObject);
            currentSelectedObject = null;
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









