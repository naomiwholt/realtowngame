using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    private InventoryItemData inventoryItem;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetItem(InventoryItemData item)
    {
        this.inventoryItem = item;

        // Set the icon to represent the item visually
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        inventoryItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        EssentialsManager._instance.objectPlacementManager.StartDrag(inventoryItem.itemPrefab);
    }

    public void OnDrag(PointerEventData eventData)
    {
        EssentialsManager._instance.objectPlacementManager.UpdateDragPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EssentialsManager._instance.objectPlacementManager.EndDrag(eventData.position, inventoryItem, this);
    }
}






