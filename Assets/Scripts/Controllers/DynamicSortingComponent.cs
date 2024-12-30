using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicSortingComponent : MonoBehaviour
{
    private Coroutine sortingCoroutine;
    public List<SpriteRenderer> SpritesToBeSorted = new List<SpriteRenderer>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        SpriteRenderer otherSprite = other.GetComponent<SpriteRenderer>();
        if (otherSprite != null && !SpritesToBeSorted.Contains(otherSprite))
        {
            SpritesToBeSorted.Add(otherSprite);

            // Start coroutine only if it's not already running
            if (sortingCoroutine == null)
            {
                sortingCoroutine = StartCoroutine(SortPeriodically());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SpriteRenderer otherSprite = other.GetComponent<SpriteRenderer>();
        if (otherSprite != null && SpritesToBeSorted.Contains(otherSprite))
        {
            SpritesToBeSorted.Remove(otherSprite);

            // Stop coroutine if no sprites left to sort
            if (SpritesToBeSorted.Count == 0 && sortingCoroutine != null)
            {
                StopCoroutine(sortingCoroutine);
                sortingCoroutine = null;
            }
        }
    }

    private IEnumerator SortPeriodically()
    {
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        DepthSortingManager depthSortingManager = EssentialsManager._instance.sortingManager;

        if (playerSprite == null || depthSortingManager == null)
        {
            sortingCoroutine = null; // End if dependencies are missing
            yield break;
        }

        while (SpritesToBeSorted.Count > 0)
        {
            playerSprite.sortingOrder = depthSortingManager.GetDynamicSortingOrder(playerSprite, SpritesToBeSorted);
            yield return new WaitForSeconds(0.1f); // Adjust interval as needed
        }

        // Coroutine ends when no sprites are left to sort
        sortingCoroutine = null;
    }
}



