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
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        // Create a slot for each item in the inventory
        foreach (GameObject prefab in inventoryManager.GetInventoryItems())
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryGrid);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();

            // Pass the prefab reference itself; do not instantiate it here
            slot.SetItem(prefab);
        }
    }
}



