using System.Collections.Generic;
using UnityEngine;

public class IsometricDepthSorting : MonoBehaviour
{
    public List<SpriteRenderer> spritesToSort = new List<SpriteRenderer>();  // List of objects to be sorted
    public Transform cameraTransform;  // The camera or player's forward direction for depth
    public SpriteRenderer player_spriterenderer;

    private void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
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

        if (player_spriterenderer != null)
        {
            AddToSortingList(player_spriterenderer);
        }
        else
        {
            Debug.LogError("Player SpriteRenderer not assigned!");
        }

        FindAndAddAllSprites();
    }

    void Update()
    {
        SortSpritesByDepthAndY();  // Continue sorting every frame
    }

    public void FindAndAddAllSprites()
    {
        spritesToSort.Clear();
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer sprite in allSprites)
        {
            AddToSortingList(sprite);
        }

        Debug.Log($"{allSprites.Length} sprites added to sorting list.");
    }

    public void SortSpritesByDepthAndY()
    {
        Vector2 depthDirection = new Vector2(cameraTransform.forward.x, cameraTransform.forward.y).normalized;

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

        for (int i = 0; i < spritesToSort.Count; i++)
        {
            spritesToSort[i].sortingOrder = i;
        }
    }

    public void AddToSortingList(SpriteRenderer sprite)
    {
        if (sprite != null && !spritesToSort.Contains(sprite))
        {
            spritesToSort.Add(sprite);
        }
    }

    public void RemoveFromSortingList(SpriteRenderer sprite)
    {
        if (spritesToSort.Contains(sprite))
        {
            spritesToSort.Remove(sprite);
        
        }
    }

    // New method to allow external scripts to request sorting updates
    public void UpdateSortingOrders(List<GameObject> objectsToSort)
    {
        foreach (GameObject obj in objectsToSort)
        {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                AddToSortingList(spriteRenderer);
            }
        }

        SortSpritesByDepthAndY();  // Sort the new list including the added objects
    }
}

