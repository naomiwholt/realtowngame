using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSortingManager : MonoBehaviour
{
    public List<SpriteRenderer> staticSprites = new List<SpriteRenderer>();  // Static objects sorted once
    public List<SpriteRenderer> dynamicSprites = new List<SpriteRenderer>(); // Dynamic objects sorted periodically

    private Coroutine dynamicSortingCoroutine;

    public void InitialiseSorting()
    {
        staticSprites.Clear();

        Transform gameWorld = GameObject.Find("Game World")?.transform; // Find the "Game World" parent
        if (gameWorld == null)
        {
            Debug.LogError("Game World parent not found!");
            return;
        }

        // Recursively collect SpriteRenderers under "Game World"
        AddChildrenSpriteRenderers(gameWorld, staticSprites);

        Debug.Log($"{staticSprites.Count} static sprites initialized for sorting.");
        AssignStaticSortingOrders();

        // Start the coroutine for dynamic sorting
        if (dynamicSortingCoroutine == null)
        {
            dynamicSortingCoroutine = StartCoroutine(SortDynamicSpritesPeriodically());
        }
    }

    // Recursively collect SpriteRenderers from children
    private void AddChildrenSpriteRenderers(Transform parent, List<SpriteRenderer> spriteList)
    {
        foreach (Transform child in parent)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteList.Add(spriteRenderer);
            }

            // Check and add from child objects
            if (child.childCount > 0)
            {
                AddChildrenSpriteRenderers(child, spriteList);
            }
        }
    }

    // Assign sorting orders to static sprites
    private void AssignStaticSortingOrders()
    {
        Debug.Log("Assigning static sorting orders...");
        staticSprites.Sort((a, b) =>
        {
            // Sort by Y position of the transform
            int yComparison = b.transform.position.y.CompareTo(a.transform.position.y);
            if (yComparison != 0) return yComparison;

            // Secondary sort by X position for consistent order
            return b.transform.position.x.CompareTo(a.transform.position.x);
        });

        // Assign sorting order
        for (int i = 0; i < staticSprites.Count; i++)
        {
            staticSprites[i].sortingOrder = i;
        }
    }

    // Add dynamic sprites (e.g., player or interactables)
    public void AddToDynamicSorting(SpriteRenderer sprite)
    {
        if (sprite != null && !dynamicSprites.Contains(sprite))
        {
            dynamicSprites.Add(sprite);
        }
    }

    // Remove dynamic sprites
    public void RemoveFromDynamicSorting(SpriteRenderer sprite)
    {
        dynamicSprites.Remove(sprite);
    }

    // Coroutine to sort dynamic sprites periodically
    private IEnumerator SortDynamicSpritesPeriodically()
    {
        while (true)
        {
            if (dynamicSprites.Count > 0)
            {
                SortDynamicSprites();
            }

            yield return new WaitForSeconds(0.1f); // Adjust interval as needed (e.g., 0.1 seconds)
        }
    }

    // Sort dynamic sprites based on bounds
    private void SortDynamicSprites()
    {
        dynamicSprites.Sort((a, b) =>
        {
            // Calculate the bottom-center point of each object
            Vector3 aBottomCenter = new Vector3(a.bounds.center.x, a.bounds.min.y, 0f);
            Vector3 bBottomCenter = new Vector3(b.bounds.center.x, b.bounds.min.y, 0f);

            // Sort by the Y position of the bottom-center (isometric depth)
            int yComparison = bBottomCenter.y.CompareTo(aBottomCenter.y);
            if (yComparison != 0) return yComparison;

            // Secondary sorting by X position (if needed)
            return bBottomCenter.x.CompareTo(aBottomCenter.x);
        });

        // Assign sorting orders, offset by static sprite count
        for (int i = 0; i < dynamicSprites.Count; i++)
        {
            dynamicSprites[i].sortingOrder = staticSprites.Count + i;
        }
    }

    private void OnDestroy()
    {
        // Stop the coroutine if the object is destroyed
        if (dynamicSortingCoroutine != null)
        {
            StopCoroutine(dynamicSortingCoroutine);
        }
    }
}





