using System.Collections.Generic;
using UnityEngine;

public class IsometricDepthSorting : MonoBehaviour
{
 public List<SpriteRenderer> spritesToSort;   // List of objects to be sorted (both dynamic and nearby static)
    public Transform cameraTransform;            // The camera or player's forward direction for depth

    void Update()
    {
        SortSpritesByDepthAndY();
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

    // Add a static object to the sorting list
    public void AddToSortingList(SpriteRenderer staticObject)
    {
        if (!spritesToSort.Contains(staticObject))
        {
            spritesToSort.Add(staticObject);
        }
    }

    // Remove a static object from the sorting list
    public void RemoveFromSortingList(SpriteRenderer staticObject)
    {
        if (spritesToSort.Contains(staticObject))
        {
            spritesToSort.Remove(staticObject);
        }
    }
}
