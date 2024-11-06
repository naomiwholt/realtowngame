using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Image icon;  // Image to display the item icon
    private GameObject furniturePrefab; // The furniture prefab associated with this slot
    private Transform originalParent;
    private Canvas canvas;
    public Sprite testsprite;

    public RectTransform inventoryPanelBoundary;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        inventoryPanelBoundary = EssentialsManager._instance.uiManager.inventoryUIPanel.GetComponent<RectTransform>();
    }

    private void Update()
    {
        //icon.sprite = testsprite;
    }

    // Set up the slot with an item prefab and dynamically retrieve its sprite
    public void SetItem(GameObject furniturePrefabFromInventory)
    {
        this.furniturePrefab = furniturePrefabFromInventory;
        //Debug.Log(furniturePrefabFromInventory.name);

        // Get the sprite from the furniture prefab's SpriteRenderer
        SpriteRenderer spriteRenderer = furniturePrefabFromInventory.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            //Debug.Log("sprite renderer on object not null");
            GetComponent<Image>().sprite = spriteRenderer.sprite;
            
           // Debug.Log(spriteRenderer.sprite + "icon sprite assigned");
            icon.enabled = true;
        }
        else
        {
            Debug.LogError($"No SpriteRenderer found on {furniturePrefabFromInventory.name}. Ensure the prefab has a SpriteRenderer with a sprite.");
            icon.enabled = false; // Hide icon if sprite is missing
        }
    }

        // Clears the slot when the item is removed
        public void ClearSlot()
        {
            furniturePrefab = null;
            icon.sprite = null;
            icon.enabled = false;
        }

        // Dragging code remains the same...



        public void OnBeginDrag(PointerEventData eventData)
        {
            originalParent = transform.parent;
            transform.SetParent(canvas.transform); // Detach to follow the mouse
            icon.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position; // Follow the cursor
        }

    public void OnEndDrag(PointerEventData eventData)
    {

        Debug.Log("Pointer Enter: " + eventData.pointerEnter);

        if (!RectTransformUtility.RectangleContainsScreenPoint(inventoryPanelBoundary, eventData.position, Camera.main))
        {
            // Convert screen position to world position
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPosition.z = 0;

            // Place the furniture at the target world position
            EssentialsManager._instance.objectPlacementManager.PlaceFurniture(furniturePrefab, worldPosition);

            // Remove the item from the inventory
            EssentialsManager._instance.inventoryManager.RemoveItem(furniturePrefab);
            Destroy(this.gameObject);
        }


        icon.raycastTarget = true;
        transform.SetParent(originalParent); // Return to slot if not placed
        
    }
}

