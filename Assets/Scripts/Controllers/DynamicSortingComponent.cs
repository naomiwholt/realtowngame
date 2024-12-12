using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class DynamicSortingComponent : MonoBehaviour
{
    private Coroutine sortingCoroutine;
    public List<SpriteRenderer> SpritesToBeSorted = new List<SpriteRenderer>(); 
    private void OnTriggerEnter2D(Collider2D other)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        SpriteRenderer otherSprite = other.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            DepthSortingManager depthSortingManager = EssentialsManager._instance.sortingManager;
            SpritesToBeSorted.Add(otherSprite);
            if (sortingCoroutine == null)
            {
                sortingCoroutine = StartCoroutine(SortPeriodically(sprite));
            }
            else
            {
               Debug.LogWarning("Sorting coroutine already running!");
            }
        }
    }



    private IEnumerator SortPeriodically(SpriteRenderer targetSprite)
    {
        DepthSortingManager depthSortingManager = EssentialsManager._instance.sortingManager;

        while (true)
        {
            if (depthSortingManager != null && targetSprite != null)
            {
                targetSprite.sortingOrder = depthSortingManager.GetDynamicSortingOrder(targetSprite, SpritesToBeSorted);
                Debug.Log("Sorting order updated for player ");
            }
            yield return new WaitForSeconds(0.1f); // Adjust interval as needed
        }
    }

    public void AddToDynamicSorting(SpriteRenderer sprite, List<SpriteRenderer> spritesToSort)
    {
        if (!spritesToSort.Contains(sprite))
        {
            spritesToSort.Add(sprite);
            Debug.Log("Dynamic sprite added for sorting. " + sprite.gameObject.name);
        }
    }

   

    private void OnTriggerExit2D(Collider2D other)
    {
        SpriteRenderer otherSprite = other.GetComponent<SpriteRenderer>();
        SpritesToBeSorted.Remove(otherSprite);   
    }
}
