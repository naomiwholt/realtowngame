using UnityEngine;

public class SortingComponent : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SortingManager sortingManager;
    private GridManager gridManager;

    public bool isStaticObject = false;  // Mark as static if it doesn’t move (e.g., buildings)
    public int sortingOrderOffset = 0;  // Allows for fine-tuning sorting

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sortingManager = FindObjectOfType<SortingManager>();
        gridManager = GridManager.Instance;

        // Register the object with the SortingManager
        sortingManager.RegisterObject(this, isStaticObject);
    }

    void OnDestroy()
    {
        // Unregister from the manager when destroyed
        sortingManager.UnregisterObject(this);
    }

    // Called by the SortingManager to update sorting order
    public void UpdateSortingOrder()
    {
        // Get the object's current grid position
        Vector2Int gridPosition = gridManager.ConvertWorldToGrid(transform.position);

        // Calculate sorting order based on Y position (higher Y is further back)
        spriteRenderer.sortingOrder = Mathf.FloorToInt(-gridPosition.y * 10) + sortingOrderOffset;
    }
}
