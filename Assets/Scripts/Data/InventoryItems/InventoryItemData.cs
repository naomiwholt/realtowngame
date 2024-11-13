using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Inventory/Item")]
public class InventoryItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon; // Icon to display in the UI
    public GameObject prefab; // Prefab to instantiate in the scene
    public string description;
    public int maxStackSize; // For stackable items

  
}

