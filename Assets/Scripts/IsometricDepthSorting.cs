using System.Collections.Generic;
using UnityEngine;

public class IsometricDepthSorting : MonoBehaviour
{
    public List<SpriteRenderer> spritesToSort = new List<SpriteRenderer>();  // List of objects to be sorted
    public Transform cameraTransform;  // The camera or player's forward direction for depth

    // Call this function explicitly to find and add all sprites in the scene

    private void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;  // Find the main camera
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                Debug.Log("CameraTransform automatically assigned to the Main Camera.");
            }
            else
            {
                Debug.LogError("No camera assigned and no Main Camera found.");
            }
        }
        FindAndAddAllSprites();
    }
    public void FindAndAddAllSprites()
    {
        // Clear the current list to avoid duplicates
        spritesToSort.Clear();

        // Find all SpriteRenderers in the scene
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer sprite in allSprites)
        {
            AddToSortingList(sprite);
        }

        Debug.Log($"{allSprites.Length} sprites added to sorting list.");
    }

    void Update()
    {
        SortSpritesByDepthAndY();  // Continue sorting every frame
    }

    // Sorts sprites by depth (dot product) and Y-position
    public void SortSpritesByDepthAndY()
    {
        Vector2 depthDirection = new Vector2(cameraTransform.forward.x, cameraTransform.forward.y).normalized;

        // Sort sprites using a combination of Y-position and depth
        spritesToSort.Sort((a, b) =>
        {
            int yComparison = b.transform.position.y.CompareTo(a.transform.position.y);
            if (yComparison != 0) return yComparison;

            Vector2 toA = new Vector2(a.transform.position.x, a.transform.position.y).normalized;
            Vector2 toB = new Vector2(b.transform.position.x, b.transform.position.y).normalized;

            float dotA = Vector2.Dot(depthDirection, toA);
            float dotB = Vector2.Dot(depthDirection, toB);

            return dotB.CompareTo(dotA);  // Inverted depth sorting if needed
        });

        // Apply sorting order to SpriteRenderers
        for (int i = 0; i < spritesToSort.Count; i++)
        {
            spritesToSort[i].sortingOrder = i;
        }
    }

    // Add a static object to the sorting list, ensuring no duplicates
    public void AddToSortingList(SpriteRenderer sprite)
    {
        if (sprite != null && !spritesToSort.Contains(sprite))
        {
            spritesToSort.Add(sprite);
        }
    }

    // Remove a static object from the sorting list
    public void RemoveFromSortingList(SpriteRenderer sprite)
    {
        if (spritesToSort.Contains(sprite))
        {
            spritesToSort.Remove(sprite);
        }
    }
}
