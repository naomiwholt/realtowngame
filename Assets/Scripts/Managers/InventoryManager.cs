using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private List<InventoryItemData> inventoryItems; // List to store items currently in the inventory

    public void Initialise()
    {
        if (inventoryItems == null)
        {
            inventoryItems = new List<InventoryItemData>();
        }
    }
    // Method to add an item to the inventory
    public void AddItem(InventoryItemData item)
    {
        if (item != null && !inventoryItems.Contains(item))
        {
            inventoryItems.Add(item);
            EssentialsManager._instance.uiManager.inventoryUI.UpdateInventoryUI();
        }
    }

    // Method to remove an item from the inventory
    public bool RemoveItem(InventoryItemData item)
    {
        if (inventoryItems == null)
        {
            Debug.LogError("Inventory items list is null.");
            return false;
        }

        if (item == null)
        {
            Debug.LogError("Item to remove is null.");
            return false;
        }

        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);

            if (EssentialsManager._instance == null)
            {
                Debug.LogError("EssentialsManager instance is null.");
            }
            else if (EssentialsManager._instance.uiManager == null)
            {
                Debug.LogError("UIManager instance is null.");
            }
            else if (EssentialsManager._instance.uiManager.inventoryUI == null)
            {
                Debug.LogError("InventoryUI instance is null.");
            }
            else
            {
                EssentialsManager._instance.uiManager.inventoryUI.UpdateInventoryUI();
            }

            return true;
        }

        return false;
    }

    // Method to retrieve a list of all items currently in the inventory
    public List<InventoryItemData> GetInventoryItems()
    {
        return new List<InventoryItemData>(inventoryItems); // Return a copy to prevent direct modification
    }
}