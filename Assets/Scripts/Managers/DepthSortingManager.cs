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



    public int GetDynamicSortingOrder(SpriteRenderer sprite, List<SpriteRenderer> spritesToCompare)
    {
        if (sprite == null || spritesToCompare == null)
            return sprite.sortingOrder;

        // Use global transform position for Y-sorting
        float playerY = sprite.transform.position.y;

        SpriteRenderer closestSprite = null;
        float smallestYDifference = float.MaxValue;

        foreach (var otherSprite in spritesToCompare)
        {
            if (otherSprite == null || otherSprite == sprite)
                continue;

            float otherY = otherSprite.transform.position.y;
            float yDifference = Mathf.Abs(playerY - otherY);

            if (yDifference < smallestYDifference)
            {
                smallestYDifference = yDifference;
                closestSprite = otherSprite;
            }
        }

        if (closestSprite == null)
            return sprite.sortingOrder;

        float closestY = closestSprite.transform.position.y;

        // Adjust sorting order based on relative Y position
        sprite.sortingOrder = (playerY < closestY)
            ? closestSprite.sortingOrder + 1 // Player is below → Render in front
            : closestSprite.sortingOrder - 1; // Player is above → Render behind

        sprite.sortingOrder += 0; // Force Unity to refresh sorting

        return sprite.sortingOrder;
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




