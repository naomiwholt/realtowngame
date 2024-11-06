using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform inventoryGrid; // Parent object with GridLayoutGroup
    public GameObject inventorySlotPrefab; // Prefab for each inventory slot

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = EssentialsManager._instance.inventoryManager;
        UpdateInventoryUI();
    }

    // Updates inventory slots based on items in InventoryManager
    public void UpdateInventoryUI()
    {
        // Clear existing slots
        //foreach (Transform child in inventoryGrid)
        //{
        //    Destroy(child.gameObject);
        //}

        // Create a slot for each item in the inventory
        foreach (GameObject item in inventoryManager.GetInventoryItems())
        {
           
            //Debug.Log(inventoryManager.inventoryItems.Count + " <--- number of inventory items");
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryGrid);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
           // Debug.Log(slot.name + " " + i);
           // Debug.Log(item.name);
            // Set item data for the slot
            //Sprite itemIcon = item.GetComponent<SpriteRenderer>().sprite;
            slot.SetItem(item);
        }
    }
}

