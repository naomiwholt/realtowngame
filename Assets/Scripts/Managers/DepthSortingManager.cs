using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSortingManager : MonoBehaviour
{
    public List<SpriteRenderer> staticSprites = new List<SpriteRenderer>();  // Static objects sorted once
    

    private Coroutine dynamicSortingCoroutine;
    private Transform furnitureParent;

    public void InitialiseSorting()
    {
        staticSprites.Clear();

        Transform gameWorld = GameObject.Find("Game World")?.transform; // Find the "Game World" parent
        furnitureParent = GameObject.Find("Furniture")?.transform; // Find the "Furniture" parent
        if (gameWorld == null || furnitureParent == null)
        {
            Debug.LogError("Game World or Furniture parent not found!");
            return;
        }

        // Recursively collect SpriteRenderers under "Game World"
        AddChildrenSpriteRenderers(gameWorld, staticSprites);

        Debug.Log($"{staticSprites.Count} static sprites initialized for sorting.");
        AssignStaticSortingOrders();

        // Start the coroutine for dynamic sorting

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




    //still need to fix this it isnt exactly working
    public int GetDynamicSortingOrder(SpriteRenderer sprite, List<SpriteRenderer> spritesToSort)
    {
        if (sprite == null || spritesToSort == null) return 0;

        // Get the bottom center point of the sprite's bounds
        Vector3 spriteBottomCenter = new Vector3(sprite.bounds.center.x, sprite.bounds.min.y, 0f);

        int closestBelowOrder = -1; // The highest sorting order below the sprite
        int closestAboveOrder = int.MaxValue; // The lowest sorting order above the sprite

        foreach (var otherSprite in spritesToSort)
        {
            if (otherSprite == null || otherSprite == sprite) continue;

            // Get the bottom center point of the other sprite's bounds
            Vector3 otherBottomCenter = new Vector3(otherSprite.bounds.center.x, otherSprite.bounds.min.y, 0f);

            if (spriteBottomCenter.y > otherBottomCenter.y)
            {
                // Sprite is above this one; track the closest order below
                closestBelowOrder = Mathf.Max(closestBelowOrder, otherSprite.sortingOrder);
            }
            else if (spriteBottomCenter.y < otherBottomCenter.y)
            {
                // Sprite is below this one; track the closest order above
                closestAboveOrder = Mathf.Min(closestAboveOrder, otherSprite.sortingOrder);
            }
        }

        // Determine the sorting order
        int sortingOrder;
        if (closestBelowOrder != -1 && closestAboveOrder != int.MaxValue)
        {
            // Place the sprite between the closest layers
            sortingOrder = closestBelowOrder + (closestAboveOrder - closestBelowOrder) / 2;
        }
        else if (closestBelowOrder != -1)
        {
            // Only layers below exist; place above the closest one
            sortingOrder = closestBelowOrder + 1;
        }
        else if (closestAboveOrder != int.MaxValue)
        {
            // Only layers above exist; place below the closest one
            sortingOrder = closestAboveOrder - 1;
        }
        else
        {
            // Default to a base sorting order if no other sprites are present
            sortingOrder = staticSprites.Count;
        }

        // Clamp the sorting order to a minimum of 0
        sortingOrder = Mathf.Max(0, sortingOrder);

        Debug.Log($"Calculated sorting order for {sprite.gameObject.name}: {sortingOrder}");
        return sortingOrder;
    }






    // Helper to check if a sprite is under the "Furniture" parent
    private bool IsUnderFurnitureParent(Transform spriteTransform)
    {
        Transform current = spriteTransform;
        while (current != null)
        {
            if (current == furnitureParent)
            {
                return true;
            }
            current = current.parent;
        }
        return false;
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




