using System.Collections.Generic;
using UnityEngine;

public class IsometricDepthSorting : MonoBehaviour
{
    public List<SpriteRenderer> spritesToSort = new List<SpriteRenderer>();  // List of objects to be sorted
    public Transform cameraTransform;  // The camera or player's forward direction for depth

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;  // Listen for game state changes
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;  // Unsubscribe to prevent memory leaks
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Playing)
        {
            InitialiseSorting();
        }
    }

    public void InitialiseSorting()
    {
        spritesToSort.Clear();

        if (ObjectPlacementManager.Instance == null)
        {
            Debug.LogError("ObjectPlacementManager instance not found!");
            return;
        }

        foreach (GameObject obj in ObjectPlacementManager.Instance.preExistingObjects)
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
    void SortSprites()
    {
        // Sort by Y position, then X for ties, with lower Y values appearing in front
        spritesToSort.Sort((a, b) =>
        {
            int yComparison = b.transform.position.y.CompareTo(a.transform.position.y); // Reverse Y comparison
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


