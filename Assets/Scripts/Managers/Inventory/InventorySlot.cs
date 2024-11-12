using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    private GameObject furniturePrefab;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetItem(GameObject furniturePrefabFromInventory)
    {
        this.furniturePrefab = furniturePrefabFromInventory;

        // Set the icon to represent the prefab visually
        SpriteRenderer spriteRenderer = furniturePrefabFromInventory.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            icon.sprite = spriteRenderer.sprite;
            icon.enabled = true;
        }
        else
        {
            Debug.LogError($"No SpriteRenderer found on {furniturePrefabFromInventory.name}. Ensure the prefab has a SpriteRenderer with a sprite.");
            icon.enabled = false;
        }
    }

    public void ClearSlot()
    {
        furniturePrefab = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        EssentialsManager._instance.objectPlacementManager.StartDrag(furniturePrefab);
    }

    public void OnDrag(PointerEventData eventData)
    {
        EssentialsManager._instance.objectPlacementManager.UpdateDragPosition(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EssentialsManager._instance.objectPlacementManager.EndDrag(eventData.position, furniturePrefab, this);
    }
}








