using System.Collections.Generic;
using UnityEngine;

public class DepthSortingManager : MonoBehaviour
{
    public List<SpriteRenderer> spritesToSort = new List<SpriteRenderer>();  // List of objects to be sorted
    public Transform cameraTransform;  // The camera or player's forward direction for depth


    public void InitialiseSorting()
    {
        spritesToSort.Clear();

        if (EssentialsManager._instance.objectPlacementManager == null)
        {
            Debug.LogError("ObjectPlacementManager instaance not found!");
            return;
        }

        foreach (GameObject obj in EssentialsManager._instance.objectPlacementManager.preExistingObjects)
        {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spritesToSort.Add(spriteRenderer);
            }
        }

        Debug.Log($"{spritesToSort.Count} sprites initialized for sorting.");
    }

    // Centralized sorting function for all sprites
    public void SortSprites()
    {
        //Debug.Log("sorting sprites");
        // Sort by Y position, then X for ties, with lower Y values appearing in front
        spritesToSort.Sort((a, b) =>
        {
            int yComparison = b.transform.position.y.CompareTo(a.transform.position.y); 
            if (yComparison != 0) return yComparison;

            return b.transform.position.x.CompareTo(a.transform.position.x); // Reverse X comparison if needed
        });

        // Assign sorting orders, with 0 being the farthest back and increasing for closer objects
        for (int i = 0; i < spritesToSort.Count; i++)
        {
            spritesToSort[i].sortingOrder = i;
        }
    }

    // Add a function to add sprites to the sorting list dynamically
    public void AddToSortingList(SpriteRenderer sprite)
    {
        if (sprite != null && !spritesToSort.Contains(sprite))
        {
            spritesToSort.Add(sprite);
        }
    }

    // Function to remove sprites from the sorting list dynamically
    public void RemoveFromSortingList(SpriteRenderer sprite)
    {
        if (spritesToSort.Contains(sprite))
        {
            spritesToSort.Remove(sprite);
        }
    }

    private void Update()
    {
        if (spritesToSort.Count > 0)
        {
            SortSprites();
        }
    }
}


