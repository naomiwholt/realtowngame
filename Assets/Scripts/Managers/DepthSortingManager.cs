using System.Collections.Generic;
using UnityEngine;

public class DepthSortingManager : MonoBehaviour
{
    public List<SpriteRenderer> staticSprites = new List<SpriteRenderer>();  // Static objects sorted once
    public List<SpriteRenderer> dynamicSprites = new List<SpriteRenderer>(); // Dynamic objects sorted in Update

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
    }

    // Recursive method to collect SpriteRenderers
    private void AddChildrenSpriteRenderers(Transform parent, List<SpriteRenderer> spriteList)
    {
        foreach (Transform child in parent)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteList.Add(spriteRenderer); // Add the SpriteRenderer to the list
            }

            // Recursive call for child objects
            if (child.childCount > 0)
            {
                AddChildrenSpriteRenderers(child, spriteList);
            }
        }
    }

    // Assign sorting orders to static sprites (once)
    private void AssignStaticSortingOrders()
    {
        staticSprites.Sort((a, b) =>
        {
            Vector3 aBottomCenter = new Vector3(a.bounds.center.x, a.bounds.min.y, 0f);
            Vector3 bBottomCenter = new Vector3(b.bounds.center.x, b.bounds.min.y, 0f);

            int yComparison = bBottomCenter.y.CompareTo(aBottomCenter.y);
            if (yComparison != 0) return yComparison;

            return bBottomCenter.x.CompareTo(aBottomCenter.x);
        });

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

    // Remove dynamic sprites when they leave the trigger
    public void RemoveFromDynamicSorting(SpriteRenderer sprite)
    {
        if (dynamicSprites.Contains(sprite))
        {
            dynamicSprites.Remove(sprite);
        }
    }

    private void Update()
    {
        // Only sort dynamic sprites in Update
        if (dynamicSprites.Count > 0)
        {
            SortDynamicSprites();
        }
    }

    //need to just fix how the gates are done so we can work this out later 
    // Sort dynamic sprites relative to static objects
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

        // Assign sorting orders, ensuring dynamic sprites stack on top of static ones
        for (int i = 0; i < dynamicSprites.Count; i++)
        {
            dynamicSprites[i].sortingOrder = staticSprites.Count + i; // Offset by static sprite count
        }
    }



}



