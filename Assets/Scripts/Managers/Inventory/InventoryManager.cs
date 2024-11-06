using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> furniturePrefabs; // List of all furniture prefabs that can be in the inventory

    public List<GameObject> inventoryItems; // List to store items currently in the inventory

    public void Initialise()
    {
        // Initialize inventory with the prefabs set in the Inspector
        inventoryItems = new List<GameObject>(furniturePrefabs);
    }

    // Method to add a furniture prefab to the inventory
    public void AddItem(GameObject furniturePrefab)
    {
        if (furniturePrefab != null && !inventoryItems.Contains(furniturePrefab))
        {
            inventoryItems.Add(furniturePrefab);
            EssentialsManager._instance.uiManager.inventoryUI.UpdateInventoryUI();
        }
    }

    // Method to remove a furniture item from the inventory
    public bool RemoveItem(GameObject furniturePrefab)
    {
        if (inventoryItems.Contains(furniturePrefab))
        {
            inventoryItems.Remove(furniturePrefab);
           // EssentialsManager._instance.uiManager.inventoryUI.UpdateInventoryUI();
            return true;
        }
        return false;
    }

    // Method to retrieve a list of all items currently in the inventory
    public List<GameObject> GetInventoryItems()
    {
        return new List<GameObject>(inventoryItems); // Return a copy to prevent direct modification
    }
}
